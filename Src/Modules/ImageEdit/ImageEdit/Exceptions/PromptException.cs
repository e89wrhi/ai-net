using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class PromptException : DomainException
{
    public PromptException(string prompt)
        : base($"prompt: '{prompt}' is invalid.")
    {
    }
}