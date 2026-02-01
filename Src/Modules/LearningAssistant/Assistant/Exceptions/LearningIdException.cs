using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class LearningIdException : DomainException
{
    public LearningIdException(Guid id)
        : base($"learning_id: '{id}' is invalid.")
    {
    }
}
