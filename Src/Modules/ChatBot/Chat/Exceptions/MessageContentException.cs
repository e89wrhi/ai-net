using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class MessageContentException : DomainException
{
    public MessageContentException(string msgcontent)
        : base($"msg_content: '{msgcontent}' is invalid.")
    {
    }
}