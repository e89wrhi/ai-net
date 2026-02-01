using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UserActionIdException : DomainException
{
    public UserActionIdException(Guid id)
        : base($"user_action_id: '{id}' is invalid.")
    {
    }
}
