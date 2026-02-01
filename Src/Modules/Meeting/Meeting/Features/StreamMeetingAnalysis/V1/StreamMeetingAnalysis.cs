using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Meeting.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public record StreamMeetingAnalysisCommand(string Transcript) : IStreamRequest<string>;

public class StreamMeetingAnalysisEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-stream",
                (StreamMeetingAnalysisRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamMeetingAnalysisCommand(request.Transcript), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamMeetingAnalysis")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Meeting Analysis")
            .WithDescription("Streams the AI analysis of a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamMeetingAnalysisRequestDto(string Transcript);

internal class StreamMeetingAnalysisHandler : IStreamRequestHandler<StreamMeetingAnalysisCommand, string>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamMeetingAnalysisHandler(MeetingDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamMeetingAnalysisCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var systemPrompt = "You are a professional meeting assistant. Analyze the transcript for a detailed summary and action items. Stream your results.";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Transcript to analyze:\n{request.Transcript}")
        };

        var fullAnalysisBuilder = new StringBuilder();
        int tokenEstimate = 0;

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullAnalysisBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist interaction after stream
        await PersistAnalysisAsync(request, fullAnalysisBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistAnalysisAsync(StreamMeetingAnalysisCommand request, string fullAnalysis, int tokenUsage, CancellationToken cancellationToken)
    {
        try 
        {
            var meetingId = MeetingId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "meeting-stream-model");
            var langCode = LanguageCode.Of("en");
            var config = new MeetingAnalysisConfiguration(true, true, langCode);

            var session = MeetingAnalysisSession.Create(meetingId, userId, modelId, config);

            var transcriptId = TranscriptId.Of(Guid.NewGuid());
            var summaryVo = TranscriptSummary.Of(fullAnalysis);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var transcript = MeetingTranscript.Create(transcriptId, request.Transcript, summaryVo, tokenCountVo, costVo);
            session.AddTranscript(transcript);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log persistence error
        }
    }
}
