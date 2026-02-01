using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class LearningNotFoundException : DomainException
{
    public LearningNotFoundException(Guid learningId)
        : base($"learning: '{learningId}' not found.")
    {
    }
}

public class LearningAlreadyExistException : DomainException
{
    public LearningAlreadyExistException(Guid learningId)
        : base($"learning: '{learningId}' already exist.")
    {
    }
}
