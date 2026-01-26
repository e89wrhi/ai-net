namespace ChatBot.Exceptions;

public class SessionIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}