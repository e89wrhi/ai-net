using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class QuizeIdException : DomainException
{
    public QuizeIdException(Guid quize_id)
        : base($"quize_id: '{quize_id}' is invalid.")
    {
    }
}
