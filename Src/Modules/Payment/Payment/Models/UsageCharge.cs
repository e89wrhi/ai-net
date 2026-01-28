using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Models;

public record UsageCharge : Entity<UsageChargeId>
{
    public SubscriptionId SubscriptionId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public string TokenUsed { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public Money Cost { get; private set; } = default!;
    public string Module { get; private set; } = default!;

    public static UsageCharge Create(UsageChargeId id, SubscriptionId subscriptionId, UserId userId, string tokenUsed, string description, Money cost, string module)
    {
        return new UsageCharge
        {
            Id = id,
            SubscriptionId = subscriptionId,
            UserId = userId,
            TokenUsed = tokenUsed,
            Description = description,
            Cost = cost,
            Module = module,
            CreatedAt = DateTime.UtcNow
        };
    }
}

