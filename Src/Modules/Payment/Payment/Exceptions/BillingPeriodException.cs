namespace Payment.Exceptions;

public class BillingPeriodException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
