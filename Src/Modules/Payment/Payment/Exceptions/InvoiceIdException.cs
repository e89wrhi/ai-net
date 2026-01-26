namespace Payment.Exceptions;

public class InvoiceIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
