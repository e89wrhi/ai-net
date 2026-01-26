namespace ImageCaption.Exceptions;

public class ObjectIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
