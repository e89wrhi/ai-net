using AI.Common.BaseExceptions;

namespace Identity.Identity.Exceptions;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string message) : base(message)
    {
    }
}