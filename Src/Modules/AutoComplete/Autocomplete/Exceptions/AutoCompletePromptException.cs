using AI.Common.BaseExceptions;

namespace AutoComplete.Exceptions;

public class AutoCompletePromptException : DomainException
{
    public AutoCompletePromptException(string prompt)
        : base($"prompt: '{prompt}' is invalid.")
    {
    }
}