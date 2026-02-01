using User.Exceptions;

namespace User.ValueObjects;

public record AnalyticsMetrics
{
    public long TotalRequests { get; }
    public long TodayRequests { get; }
    public long WeekRequests { get; }
    public long MonthRequests { get; }

    public AnalyticsMetrics(long total, long today, long week, long month)
    {
        TotalRequests = total;
        TodayRequests = today;
        WeekRequests = week;
        MonthRequests = month;
    }
}
