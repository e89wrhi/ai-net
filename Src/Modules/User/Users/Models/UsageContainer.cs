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

    private UsageContainer() { }

    public static UsageContainer Create(UsageContainerId id, UserId userId, UsagePeriod period, string tokenUsed)
    {
        return new UsageContainer
        {
            Id = id,
            UserId = userId,
            Period = period,
            TokenUsed = tokenUsed,
            RequestsCount = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void IncrementRequests()
    {
        RequestsCount++;
        LastModified = DateTime.UtcNow;
    }
}
