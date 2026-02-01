using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Summary.ValueObjects;

namespace Summary.Models;

public record TextSummaryResult : Entity<SummaryResultId>
{
    public string OriginalText { get; private set; } = default!;
    public SummaryText Summary { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime SummarizedAt { get; private set; }

    private TextSummaryResult() { }

    public static TextSummaryResult Create(
        SummaryResultId id,
        string originalText,
        SummaryText summary,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new TextSummaryResult
        {
            Id = id,
            OriginalText = originalText,
            Summary = summary,
            TokenUsed = tokenUsed,
            Cost = cost,
            SummarizedAt = DateTime.UtcNow
        };
    }
}
