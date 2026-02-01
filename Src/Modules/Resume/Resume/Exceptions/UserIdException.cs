using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class UserIdException : DomainException
{
    public UserIdException(Guid id)
        : base($"user_id: '{id}' is invalid.")
    {
    }
}
