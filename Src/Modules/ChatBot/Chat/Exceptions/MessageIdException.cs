using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class MessageIdException : DomainException
{
    public MessageIdException(Guid messageId)
        : base($"message_id: '{messageId}' is invalid.")
    {
    }
}