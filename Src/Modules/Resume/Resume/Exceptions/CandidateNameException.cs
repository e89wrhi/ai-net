namespace Resume.Exceptions;

public class CandidateNameException : DomainException
{
    public InvalidTimeException(DateTime time)
        : base($"time: '{time}' is invalid.")
    {
    }
}
