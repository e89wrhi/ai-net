using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class QuizNotFoundException : DomainException
{
    public QuizNotFoundException(Guid quiz)
        : base($"quiz: '{quiz}' is invalid.")
    {
    }
}