namespace ImageCaption.Exceptions;

public class ObjectException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
