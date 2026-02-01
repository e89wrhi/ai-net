using User.Exceptions;

namespace User.ValueObjects;

public record UsageMetrics
{
    public int ActionsCount { get; }
    public TimeSpan TotalTimeSpent { get; }

    public UsageMetrics(int actionsCount, TimeSpan totalTimeSpent)
    {
        ActionsCount = actionsCount;
        TotalTimeSpent = totalTimeSpent;
    }
}
