using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SuggestionException : DomainException
{
    public SuggestionException(string suggestion)
        : base($"suggestion: '{suggestion}' is invalid.")
    {
    }
}
