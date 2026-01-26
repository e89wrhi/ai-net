using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class QuizeException : DomainException
{
    public QuizeException(string quize)
        : base($"quize: '{quize}' is invalid.")
    {
    }
}
