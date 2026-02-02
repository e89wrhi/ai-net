using AiOrchestration.ValueObjects;
using MediatR;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace Meeting.Features.StreamMeetingAnalysis.V1;


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
