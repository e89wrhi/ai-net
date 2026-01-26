namespace ImageCaption.Exceptions;

public class CaptionException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
