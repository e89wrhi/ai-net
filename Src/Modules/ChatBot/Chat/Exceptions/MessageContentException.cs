using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class MessageContentException : DomainException
{
    public MessageContentException(string msgcontent)
        : base($"msgcontent: '{msgcontent}' is invalid.")
    {
    }
}