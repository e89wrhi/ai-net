namespace LearningAssistant.Exceptions;

public class QuizeIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
