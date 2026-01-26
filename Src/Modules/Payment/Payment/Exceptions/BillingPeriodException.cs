using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class BillingPeriodException : DomainException
{
    public BillingPeriodException(string billing_period)
        : base($"billing_period: '{billing_period}' is invalid.")
    {
    }
}
