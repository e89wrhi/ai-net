using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UsageContainer : Entity<UsageContainerId>
{
    public UserId UserId { get; private set; } = default!;
    // Daily/Monthly
    public UsagePeriod Period { get; private set; } = default!;
    public string TokenUsed { get; private set; } = default!;
    public int RequestsCount { get; private set; } = default!;
}
