namespace LearningAssistant.Exceptions;

public class QuizeException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
