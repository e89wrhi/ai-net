using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Models;

public record LearningSession : Aggregate<LearningId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public LearningSessionStatus Status { get; private set; }
    public LearningConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastInteractionAt { get; private set; }
    public double? Score { get; private set; }

    private readonly List<LearningActivity> _activities = new();
    public IReadOnlyCollection<LearningActivity> Activities => _activities.AsReadOnly();

    private LearningSession() { }

    public static LearningSession Create(
        LearningId id,
        UserId userId,
        ModelId aiModelId,
        LearningConfiguration configuration)
    {
        var session = new LearningSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = LearningSessionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastInteractionAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.LearningSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddActivity(LearningActivity activity)
    {
        if (Status != LearningSessionStatus.Active)
            throw new DomainException("Learning session is not active.");

        _activities.Add(activity);
        LastInteractionAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += activity.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += activity.Cost);
        AddDomainEvent(
            new Events.LearningActivityRecordedDomainEvent(
                Id, activity.Id, activity.Topic.Value));
    }

    public void Complete()
    {
        Status = LearningSessionStatus.Completed;
        AddDomainEvent(new Events.LearningSessionCompletedDomainEvent(Id));
    }

    public void Fail(LearningFailureReason reason)
    {
        Status = LearningSessionStatus.Failed;
        AddDomainEvent(new Events.LearningSessionFailedDomainEvent(Id, reason));
    }

    public void SubmitScore(double score)
    {
        if (Status != LearningSessionStatus.Active)
            throw new DomainException("Learning session is not active.");
        Score = score;
        AddDomainEvent(new Events.LearningSessionCompletedDomainEvent(Id));
    }
}
