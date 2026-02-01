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

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptCommand(string Transcript) : ICommand<AnalyzeMeetingTranscriptResult>;

public record AnalyzeMeetingTranscriptResult(Guid MeetingId, Guid TranscriptId, string Summary);

public class AnalyzeMeetingTranscriptEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-transcript",
                async (AnalyzeMeetingTranscriptRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeMeetingTranscriptCommand(request.Transcript);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeMeetingTranscriptResponseDto(result.MeetingId, result.TranscriptId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeMeetingTranscript")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<AnalyzeMeetingTranscriptResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Meeting Transcript")
            .WithDescription("Uses AI to summarize a meeting transcript and extract key insights.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeMeetingTranscriptRequestDto(string Transcript);
public record AnalyzeMeetingTranscriptResponseDto(Guid MeetingId, Guid TranscriptId, string Summary);

internal class AnalyzeMeetingTranscriptHandler : ICommandHandler<AnalyzeMeetingTranscriptCommand, AnalyzeMeetingTranscriptResult>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeMeetingTranscriptHandler(MeetingDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeMeetingTranscriptResult> Handle(AnalyzeMeetingTranscriptCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var systemPrompt = "You are a meeting assistant. Summarize the provided transcript into a concise summary and extract key action items and decisions.";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Transcript:\n{request.Transcript}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var summaryText = completion.Message.Text ?? "Failed to analyze transcript.";

        // Persist
        var meetingId = MeetingId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "meeting-model");
        
        // Note: Check MeetingAnalysisConfiguration constructor
        var langCode = LanguageCode.Of("en");
        var config = new MeetingAnalysisConfiguration(true, true, langCode);

        var session = MeetingAnalysisSession.Create(meetingId, userId, modelId, config);

        var transcriptId = TranscriptId.Of(Guid.NewGuid());
        var summaryVo = TranscriptSummary.Of(summaryText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var transcript = MeetingTranscript.Create(transcriptId, request.Transcript, summaryVo, tokenCountVo, costVo);
        session.AddTranscript(transcript);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeMeetingTranscriptResult(meetingId.Value, transcriptId.Value, summaryText);
    }
}
