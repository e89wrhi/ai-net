using AI.Common.BaseExceptions;

namespace ImageGen.Exceptions;

public class PromptException : DomainException
{
    public PromptException(string prompt)
        : base($"prompt: '{prompt}' is invalid.")
    {
    }
}