using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ExpirenceIdException : DomainException
{
    public ExpirenceIdException(Guid expirence_id)
        : base($"expirence_id: '{expirence_id}' is invalid.")
    {
    }
}
