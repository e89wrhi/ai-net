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

namespace Meeting.Features.ExtractActionItems.V1;

public record ExtractActionItemsCommand(string Transcript) : ICommand<ExtractActionItemsResult>;

public record ExtractActionItemsResult(Guid MeetingId, string ActionItems);

public class ExtractActionItemsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/action-items",
                async (ExtractActionItemsRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new ExtractActionItemsCommand(request.Transcript);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ExtractActionItemsResponseDto(result.MeetingId, result.ActionItems));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ExtractActionItems")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<ExtractActionItemsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Extract Meeting Action Items")
            .WithDescription("Uses AI to extract only the actionable tasks and assignments from a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record ExtractActionItemsRequestDto(string Transcript);
public record ExtractActionItemsResponseDto(Guid MeetingId, string ActionItems);

internal class ExtractActionItemsHandler : ICommandHandler<ExtractActionItemsCommand, ExtractActionItemsResult>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public ExtractActionItemsHandler(MeetingDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ExtractActionItemsResult> Handle(ExtractActionItemsCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an efficiency expert. Extract a numbered list of ONLY specific action items, the responsible person (if mentioned), and any deadlines from the transcript."),
            new ChatMessage(ChatRole.User, $"Transcript:\n{request.Transcript}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var actionItems = completion.Message.Text ?? "No action items found.";

        // Persist
        var meetingId = MeetingId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); 
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "action-item-model");
        var langCode = LanguageCode.Of("en");
        var config = new MeetingAnalysisConfiguration(true, false, langCode);

        var session = MeetingAnalysisSession.Create(meetingId, userId, modelId, config);

        var transcriptId = TranscriptId.Of(Guid.NewGuid());
        var summaryVo = TranscriptSummary.Of(actionItems); // Using Summary VO to store action items for now
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var transcript = MeetingTranscript.Create(transcriptId, request.Transcript, summaryVo, tokenCountVo, costVo);
        session.AddTranscript(transcript);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ExtractActionItemsResult(meetingId.Value, actionItems);
    }
}
