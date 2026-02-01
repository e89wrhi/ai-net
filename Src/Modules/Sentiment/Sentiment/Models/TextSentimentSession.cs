using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Sentiment.Enums;
using Sentiment.ValueObjects;

namespace Sentiment.Models;

public record TextSentimentSession : Aggregate<SentimentId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public TextSentimentStatus Status { get; private set; }
    public TextSentimentConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastAnalyzedAt { get; private set; }

    private readonly List<TextSentimentResult> _results = new();
    public IReadOnlyCollection<TextSentimentResult> Results => _results.AsReadOnly();

    private TextSentimentSession() { }

    public static TextSentimentSession Create(
        SentimentId id,
        UserId userId,
        ModelId aiModelId,
        TextSentimentConfiguration configuration)
    {
        var session = new TextSentimentSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = TextSentimentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastAnalyzedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.TextSentimentSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(TextSentimentResult result)
    {
        if (Status != TextSentimentStatus.Active)
            throw new DomainException("Text sentiment session is not active.");

        _results.Add(result);
        LastAnalyzedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.TextSentimentAnalyzedDomainEvent(
                Id, result.Id, result.Sentiment));
    }

    public void Complete()
    {
        Status = TextSentimentStatus.Completed;
        AddDomainEvent(new Events.TextSentimentSessionCompletedDomainEvent(Id));
    }

    public void Fail(TextSentimentFailureReason reason)
    {
        Status = TextSentimentStatus.Failed;
        AddDomainEvent(new Events.TextSentimentSessionFailedDomainEvent(Id, reason));
    }
}
