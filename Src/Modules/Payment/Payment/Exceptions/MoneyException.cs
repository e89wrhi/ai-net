namespace Payment.Exceptions;

public class MoneyException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
