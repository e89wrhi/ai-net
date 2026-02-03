using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace Meeting.Features.ExtractActionItems.V1;


internal class ExtractActionItemsHandler : ICommandHandler<ExtractActionItemsCommand, ExtractActionItemsCommandResult>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public ExtractActionItemsHandler(MeetingDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ExtractActionItemsCommandResult> Handle(ExtractActionItemsCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an efficiency expert. Extract a numbered list of ONLY specific action items, the responsible person (if mentioned), and any deadlines from the transcript."),
            new ChatMessage(ChatRole.User, $"Transcript:\n{request.Transcript}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var actionItems = completion.Messages[0].Text ?? "No action items found.";

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

        return new ExtractActionItemsCommandResult(meetingId.Value, actionItems);
    }
}
