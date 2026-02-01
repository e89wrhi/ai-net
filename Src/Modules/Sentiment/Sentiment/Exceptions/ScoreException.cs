using AI.Common.BaseExceptions;

namespace Sentiment.Exceptions;

public class ScoreException : DomainException
{
    public ScoreException(double score)
        : base($"sentiment_score: '{score}' is invalid.")
    {
    }
}