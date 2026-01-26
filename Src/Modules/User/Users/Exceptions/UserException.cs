using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"user: '{userId}' not found.")
    {
    }
}

public class UserAlreadyExistException : DomainException
{
    public UserAlreadyExistException(Guid userId)
        : base($"user: '{userId}' already exist.")
    {
    }
}
