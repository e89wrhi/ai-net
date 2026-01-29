using Payment.Exceptions;

namespace Payment.ValueObjects;

public record PlanLimits
{
    public int MaxRequestsPerDay { get; }
    public int MaxTokensPerMonth { get; }

    private PlanLimits(int maxRequestsPerDay, int maxTokensPerMonth)
    {
        MaxRequestsPerDay = maxRequestsPerDay;
        MaxTokensPerMonth = maxTokensPerMonth;
    }

    public static PlanLimits Of(int maxRequestsPerDay, int maxTokensPerMonth)
    {
        if (maxTokensPerMonth < 0 || maxRequestsPerDay < 0)
        {
            throw new PlanLimitException(maxRequestsPerDay, maxTokensPerMonth);
        }

        return new PlanLimits(maxRequestsPerDay, maxTokensPerMonth);
    }

    public static implicit operator int(PlanLimits @value)
    {
        return value.MaxRequestsPerDay;
    }
}