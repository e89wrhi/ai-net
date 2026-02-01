using AI.Common.Core;
using AiOrchestration.ValueObjects;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Models;

public record TextToSpeechResult : Entity<TextToSpeechResultId>
{
    public string OriginalText { get; private set; } = default!;
    public SynthesizedSpeech Speech { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime SynthesizedAt { get; private set; }

    private TextToSpeechResult() { }

    public static TextToSpeechResult Create(
        TextToSpeechResultId id,
        string originalText,
        SynthesizedSpeech speech,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new TextToSpeechResult
        {
            Id = id,
            OriginalText = originalText,
            Speech = speech,
            TokenUsed = tokenUsed,
            Cost = cost,
            SynthesizedAt = DateTime.UtcNow
        };
    }
}

