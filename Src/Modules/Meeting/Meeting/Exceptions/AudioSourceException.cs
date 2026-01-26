namespace Meeting.Exceptions;

public class AudioSourceException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
