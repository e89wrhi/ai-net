using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Summary.Enums;
using Summary.ValueObjects;

namespace Summary.Models;

public record TextSummarySession : Aggregate<SummaryId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public TextSummaryStatus Status { get; private set; }
    public TextSummaryConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastSummarizedAt { get; private set; }

    private readonly List<TextSummaryResult> _results = new();
    public IReadOnlyCollection<TextSummaryResult> Results => _results.AsReadOnly();

    private TextSummarySession() { }

    public static TextSummarySession Create(
        SummaryId id,
        UserId userId,
        ModelId aiModelId,
        TextSummaryConfiguration configuration)
    {
        var session = new TextSummarySession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = TextSummaryStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastSummarizedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.TextSummarySessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(TextSummaryResult result)
    {
        if (Status != TextSummaryStatus.Active)
            throw new DomainException("Text summary session is not active.");

        _results.Add(result);
        LastSummarizedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);
        AddDomainEvent(
            new Events.TextSummarizedDomainEvent(
                Id, result.Id, result.Summary.Value));
    }

    public void Complete()
    {
        Status = TextSummaryStatus.Completed;
        AddDomainEvent(new Events.TextSummarySessionCompletedDomainEvent(Id));
    }

    public void Fail(TextSummaryFailureReason reason)
    {
        Status = TextSummaryStatus.Failed;
        AddDomainEvent(new Events.TextSummarySessionFailedDomainEvent(Id, reason));
    }
}
