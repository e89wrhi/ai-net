using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

internal class AnalyzeMeetingTranscriptHandler : ICommandHandler<AnalyzeMeetingTranscriptCommand, AnalyzeMeetingTranscriptCommandResult>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public AnalyzeMeetingTranscriptHandler(MeetingDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeMeetingTranscriptCommandResult> Handle(AnalyzeMeetingTranscriptCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var systemPrompt = "You are a meeting assistant. Summarize the provided transcript into a concise summary and extract key action items and decisions.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Transcript:\n{request.Transcript}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var summaryText = completion.Messages[0].Text ?? "Failed to analyze transcript.";

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

        return new AnalyzeMeetingTranscriptCommandResult(meetingId.Value, transcriptId.Value, summaryText);
    }
}

