using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class SessionIdException : DomainException
{
    public SessionIdException(Guid session_id)
        : base($"session_id: '{session_id}' is invalid.")
    {
    }
}