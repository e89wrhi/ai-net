using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class CandidateScoreException : DomainException
{
    public CandidateScoreException(double score)
        : base($"candidate_score: '{score}' is invalid.")
    {
    }
}
