using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserAnalytics : Aggregate<UserAnalyticsId>
{
    public UserId User { get; private set; } = default!;
    public long TotalRequests { get; private set; }
    public long TodayRequests { get; private set; }
    public long WeekRequests { get; private set; }
    public long MonthRequests { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private UserAnalytics() { }

    public static UserAnalytics Create(UserAnalyticsId id, UserId user)
    {
        return new UserAnalytics
        {
            Id = id,
            User = user,
            TotalRequests = 0,
            TodayRequests = 0,
            WeekRequests = 0,
            MonthRequests = 0,
            LastUpdatedAt = DateTime.UtcNow
        };
    }

    public void Increment(DateTime timestamp)
    {
        TotalRequests++;

        if (timestamp.Date == DateTime.UtcNow.Date)
            TodayRequests++;

        var diffDays = (DateTime.UtcNow - timestamp).TotalDays;
        if (diffDays < 7)
            WeekRequests++;

        if (diffDays < 30)
            MonthRequests++;

        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ResetDaily() => TodayRequests = 0;
    public void ResetWeekly() => WeekRequests = 0;
    public void ResetMonthly() => MonthRequests = 0;
}
