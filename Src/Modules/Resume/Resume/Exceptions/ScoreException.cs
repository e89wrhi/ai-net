using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ScoreException : DomainException
{
    public ScoreException(double score)
        : base($"score: '{score}' is invalid.")
    {
    }
}
