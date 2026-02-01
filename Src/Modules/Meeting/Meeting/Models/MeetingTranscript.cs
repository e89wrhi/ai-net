using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Meeting.ValueObjects;

namespace Meeting.Models;

public record MeetingTranscript : Entity<TranscriptId>
{
    public string RawTranscript { get; private set; } = default!;
    public TranscriptSummary Summary { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime RecordedAt { get; private set; }

    private MeetingTranscript() { }

    public static MeetingTranscript Create(
        TranscriptId id,
        string rawTranscript,
        TranscriptSummary summary,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new MeetingTranscript
        {
            Id = id,
            RawTranscript = rawTranscript,
            Summary = summary,
            TokenUsed = tokenUsed,
            Cost = cost,
            RecordedAt = DateTime.UtcNow
        };
    }
}
