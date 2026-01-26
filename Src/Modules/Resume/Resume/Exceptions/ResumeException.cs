using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ResumeException : DomainException
{
    public ResumeException(string resume)
        : base($"resume: '{resume}' is invalid.")
    {
    }
}
