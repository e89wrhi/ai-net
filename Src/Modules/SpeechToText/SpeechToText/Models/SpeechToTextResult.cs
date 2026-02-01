using AI.Common.Core;
using AiOrchestration.ValueObjects;
using SpeechToText.ValueObjects;

namespace SpeechToText.Models;

public record SpeechToTextResult : Entity<SpeechToTextResultId>
{
    public AudioSource Audio { get; private set; } = default!;
    public Transcript Transcript { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime TranscribedAt { get; private set; }

    private SpeechToTextResult() { }

    public static SpeechToTextResult Create(
        SpeechToTextResultId id,
        AudioSource audio,
        Transcript transcript,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new SpeechToTextResult
        {
            Id = id,
            Audio = audio,
            Transcript = transcript,
            TokenUsed = tokenUsed,
            Cost = cost,
            TranscribedAt = DateTime.UtcNow
        };
    }
}
