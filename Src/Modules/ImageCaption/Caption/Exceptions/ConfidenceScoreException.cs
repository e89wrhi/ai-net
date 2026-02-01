using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ConfidenceScoreException : DomainException
{
    public ConfidenceScoreException(double score)
        : base($"confidence_score: '{score}' is invalid.")
    {
    }
}
