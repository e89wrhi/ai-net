namespace ChatBot.Exceptions;

public class MessageSenderException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}