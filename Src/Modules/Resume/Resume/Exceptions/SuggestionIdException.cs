using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SuggestionIdException : DomainException
{
    public SuggestionIdException(Guid suggestion_id)
        : base($"suggestion_id: '{suggestion_id}' is invalid.")
    {
    }
}
