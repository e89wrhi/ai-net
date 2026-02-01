using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class UserIdException : DomainException
{
    public UserIdException(Guid user_id)
        : base($"user_id: '{user_id}' is invalid.")
    {
    }
}
