namespace Meeting.Exceptions;

public class ActionException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
