using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class QuizIdException : DomainException
{
    public QuizIdException(Guid quiz_id)
        : base($"quiz_id: '{quiz_id}' is invalid.")
    {
    }
}
