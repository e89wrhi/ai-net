using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Translate.ValueObjects;

namespace Translate.Models;

public record TranslationResult : Entity<TranslateResultId>
{
    public string OriginalText { get; private set; } = default!;
    public TranslatedText TranslatedText { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime TranslatedAt { get; private set; }

    private TranslationResult() { }

    public static TranslationResult Create(
        TranslateResultId id,
        string originalText,
        TranslatedText translatedText,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new TranslationResult
        {
            Id = id,
            OriginalText = originalText,
            TranslatedText = translatedText,
            TokenUsed = tokenUsed,
            Cost = cost,
            TranslatedAt = DateTime.UtcNow
        };
    }
}
