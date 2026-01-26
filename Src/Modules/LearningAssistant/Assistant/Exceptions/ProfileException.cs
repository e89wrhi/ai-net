namespace LearningAssistant.Exceptions;

public class ProfileException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
