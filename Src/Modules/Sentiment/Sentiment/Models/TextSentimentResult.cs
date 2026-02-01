using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Sentiment.ValueObjects;

namespace Sentiment.Models;

public record TextSentimentResult : Entity<SentimentResultId>
{
    public string Text { get; private set; } = default!;
    public SentimentText Sentiment { get; private set; }
    public SentimentScore Score { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime AnalyzedAt { get; private set; }

    private TextSentimentResult() { }

    public static TextSentimentResult Create(
        SentimentResultId id,
        string text,
        SentimentText sentiment,
        SentimentScore score,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new TextSentimentResult
        {
            Id = id,
            Text = text,
            Sentiment = sentiment,
            Score = score,
            TokenUsed = tokenUsed,
            Cost = cost,
            AnalyzedAt = DateTime.UtcNow
        };
    }
}
