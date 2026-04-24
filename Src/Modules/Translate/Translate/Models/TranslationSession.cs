using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Translate.Enums;
using Translate.ValueObjects;

namespace Translate.Models;

public record TranslationSession : Aggregate<TranslateId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public TranslationStatus Status { get; private set; }
    public TranslationConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastTranslatedAt { get; private set; }

    private readonly List<TranslationResult> _results = new();
    public IReadOnlyCollection<TranslationResult> Results => _results.AsReadOnly();

    private TranslationSession() 
    { 
        _results = new();
    }

    public static TranslationSession Create(
        TranslateId id,
        UserId userId,
        ModelId aiModelId,
        TranslationConfiguration configuration)
    {
        var session = new TranslationSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = TranslationStatus.Active,
            TotalTokens = TokenCount.Of(0),
            TotalCost = CostEstimate.Of(0),
            CreatedAt = DateTime.UtcNow,
            LastTranslatedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.TranslationSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(TranslationResult result)
    {
        if (Status != TranslationStatus.Active)
            throw new DomainException("Translation session is not active.");

        _results.Add(result);
        LastTranslatedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.TextTranslatedDomainEvent(
                Id, result.Id, result.TranslatedText.Value));
    }

    public void Complete()
    {
        Status = TranslationStatus.Completed;
        AddDomainEvent(new Events.TranslationSessionCompletedDomainEvent(Id));
    }

    public void Fail(TranslationFailureReason reason)
    {
        Status = TranslationStatus.Failed;
        AddDomainEvent(new Events.TranslationSessionFailedDomainEvent(Id, reason));
    }
}
