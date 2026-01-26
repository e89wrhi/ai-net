namespace LearningAssistant.Exceptions;

public class LessonException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
