namespace ChatBot.Exceptions;

public class MessageTimeException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}