using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UserActivityException : DomainException
{
    public UserActivityException(string user_activity)
        : base($"user_activity: '{user_activity}' is invalid.")
    {
    }
}
