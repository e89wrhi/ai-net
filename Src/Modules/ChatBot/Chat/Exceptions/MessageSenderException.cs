using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class MessageSenderException : DomainException
{
    public MessageSenderException(string sender)
        : base($"sender: '{sender}' is invalid.")
    {
    }
}