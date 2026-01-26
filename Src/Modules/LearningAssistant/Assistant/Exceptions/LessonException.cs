using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class LessonNotFoundException : DomainException
{
    public LessonNotFoundException(Guid lessonId)
        : base($"lesson: '{lessonId}' not found.")
    {
    }
}

public class LessonAlreadyExistException : DomainException
{
    public LessonAlreadyExistException(Guid lessonId)
        : base($"lesson: '{lessonId}' already exist.")
    {
    }
}
