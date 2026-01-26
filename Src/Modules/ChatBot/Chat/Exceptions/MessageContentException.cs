namespace ChatBot.Exceptions;

public class MessageContentException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}