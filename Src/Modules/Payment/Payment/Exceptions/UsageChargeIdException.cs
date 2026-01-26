namespace Payment.Exceptions;

public class UsageChargeIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
