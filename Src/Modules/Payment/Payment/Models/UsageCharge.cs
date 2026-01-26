using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Models;

public record UsageCharge : Entity<UsageChargeId>
{
    public SubscriptionId SubscriptionId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public string TokenUsed { get; private set; } = default!;
    public Money Cost { get; private set; } = default!;
    public string Module { get; private set; } = default!;

}
