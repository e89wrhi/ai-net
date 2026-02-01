using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class SystemPromptException : DomainException
{
    public SystemPromptException(string prompt)
        : base($"prompt: '{prompt}' is invalid.")
    {
    }
}