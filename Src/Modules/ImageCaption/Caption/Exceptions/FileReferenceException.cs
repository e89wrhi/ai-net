namespace ImageCaption.Exceptions;

public class FileReferenceException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
