using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class LessonIdException : DomainException
{
    public LessonIdException(Guid lesson_id)
        : base($"lesson_id: '{lesson_id}' is invalid.")
    {
    }
}
