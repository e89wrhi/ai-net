using AI.Common.BaseExceptions;

namespace AutoComplete.Exceptions;

public class AutoCompleteSuggestionException : DomainException
{
    public AutoCompleteSuggestionException(string suggestion)
        : base($"suggestion: '{suggestion}' is invalid.")
    {
    }
}