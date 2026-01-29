using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ResumeNotFoundException : DomainException
{
    public ResumeNotFoundException(Guid id)
        : base($"resume_id: '{id}' is invalid.")
    {
    }
}
