using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using SpeechToText.Enums;
using SpeechToText.ValueObjects;

namespace SpeechToText.Models;

public record SpeechToTextSession : Aggregate<SpeechToTextId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public SpeechToTextStatus Status { get; private set; }
    public SpeechToTextConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastTranscribedAt { get; private set; }

    private readonly List<SpeechToTextResult> _results = new();
    public IReadOnlyCollection<SpeechToTextResult> Results => _results.AsReadOnly();

    private SpeechToTextSession() { }

    public static SpeechToTextSession Create(
        SpeechToTextId id,
        UserId userId,
        ModelId aiModelId,
        SpeechToTextConfiguration configuration)
    {
        var session = new SpeechToTextSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = SpeechToTextStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastTranscribedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.SpeechToTextSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(SpeechToTextResult result)
    {
        if (Status != SpeechToTextStatus.Active)
            throw new DomainException("Speech-to-text session is not active.");

        _results.Add(result);
        LastTranscribedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.SpeechTranscribedDomainEvent(
                Id, result.Id, result.Transcript.Value));
    }

    public void Complete()
    {
        Status = SpeechToTextStatus.Completed;
        AddDomainEvent(new Events.SpeechToTextSessionCompletedDomainEvent(Id));
    }

    public void Fail(SpeechToTextFailureReason reason)
    {
        Status = SpeechToTextStatus.Failed;
        AddDomainEvent(new Events.SpeechToTextSessionFailedDomainEvent(Id, reason));
    }
}
