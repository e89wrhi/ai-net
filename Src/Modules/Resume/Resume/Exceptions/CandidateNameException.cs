using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class CandidateNameException : DomainException
{
    public CandidateNameException(string candidate_name)
        : base($"candidate_name: '{candidate_name}' is invalid.")
    {
    }
}
