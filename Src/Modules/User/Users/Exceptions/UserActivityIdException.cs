using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UserActivityIdException : DomainException
{
    public UserActivityIdException(Guid user_activity_id)
        : base($"user_activity_id: '{user_activity_id}' is invalid.")
    {
    }
}
