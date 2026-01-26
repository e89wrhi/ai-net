using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class PlanLimitException : DomainException
{
    public PlanLimitException(string plan_limit)
        : base($"plan_limit: '{plan_limit}' is invalid.")
    {
    }
}
