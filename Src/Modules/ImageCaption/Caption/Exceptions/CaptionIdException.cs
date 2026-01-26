namespace ImageCaption.Exceptions;

public class CaptionIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
