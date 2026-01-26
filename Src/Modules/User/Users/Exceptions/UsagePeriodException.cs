using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UsagePeriodException : DomainException
{
    public UsagePeriodException(string usage_period)
        : base($"usage_period: '{usage_period}' is invalid.")
    {
    }
}
