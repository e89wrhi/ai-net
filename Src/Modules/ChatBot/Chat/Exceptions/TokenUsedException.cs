namespace ChatBot.Exceptions;

public class TokenUsedException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
