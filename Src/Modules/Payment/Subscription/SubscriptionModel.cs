using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record Subscription : Entity<SubscriptionId>
{
    public UserId UserId { get; private set; } = default!;
    public SubscriptionPlan Plan { get; private set; } = default!;
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndsAt { get; private set; }

    private Subscription() { }

    public static Subscription Create(UserId userId, SubscriptionPlan plan)
    {
        return new Subscription
        {
            Id = SubscriptionId.New(),
            UserId = userId,
            Plan = plan,
            Status = SubscriptionStatus.Active,
            StartedAt = DateTime.UtcNow
        };
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        EndsAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        Status = SubscriptionStatus.Paused;
    }

    public void Resume()
    {
        Status = SubscriptionStatus.Active;
    }
}
