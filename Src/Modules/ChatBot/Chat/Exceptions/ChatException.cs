using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class ChatNotFoundException : DomainException
{
    public ChatNotFoundException(Guid chatId)
        : base($"chat: '{chatId}' not found.")
    {
    }
}

public class ChatAlreadyExistException : DomainException
{
    public ChatAlreadyExistException(Guid chatId)
        : base($"chat: '{chatId}' already exist.")
    {
    }
}
