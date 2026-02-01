using AI.Common.BaseExceptions;

namespace Translate.Exceptions;

public class TranslateNotFoundException : DomainException
{
    public TranslateNotFoundException(Guid translateId)
        : base($"translate: '{translateId}' not found.")
    {
    }
}

public class TranslateAlreadyExistException : DomainException
{
    public TranslateAlreadyExistException(Guid translateId)
        : base($"translate: '{translateId}' already exist.")
    {
    }
}
