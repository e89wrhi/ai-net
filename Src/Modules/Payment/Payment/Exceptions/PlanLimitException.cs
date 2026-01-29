using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class PlanLimitException : DomainException
{
    public PlanLimitException(int maxRequestsPerDay, int maxTokensPerMonth)
        : base($"requests per day: '{maxRequestsPerDay}'/tokens per month: '{maxTokensPerMonth}' is invalid.")
    {
    }
}
