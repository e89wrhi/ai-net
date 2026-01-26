using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ExpirenceException : DomainException
{
    public ExpirenceException(string expirence)
        : base($"expirence: '{expirence}' is invalid.")
    {
    }
}
