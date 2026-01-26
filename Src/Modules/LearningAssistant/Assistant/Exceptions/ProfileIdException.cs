namespace LearningAssistant.Exceptions;

public class ProfileIdException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
