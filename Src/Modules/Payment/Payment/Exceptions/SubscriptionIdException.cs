namespace Payment.Exceptions;

public class SubscriptionIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
