using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ResumeIdException : DomainException
{
    public ResumeIdException(Guid resume_id)
        : base($"resume_id: '{resume_id}' is invalid.")
    {
    }
}
