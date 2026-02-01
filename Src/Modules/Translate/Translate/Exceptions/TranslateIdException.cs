using AI.Common.BaseExceptions;

namespace Translate.Exceptions;

public class TranslateIdException : DomainException
{
    public TranslateIdException(Guid id)
        : base($"translate_id: '{id}' is invalid.")
    {
    }
}