using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class MessageTimeException : DomainException
{
    public MessageTimeException(DateTime msg_session_time)
        : base($"msg_session_time: '{msg_session_time}' is invalid.")
    {
    }
}