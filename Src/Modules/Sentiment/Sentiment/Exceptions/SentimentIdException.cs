using AI.Common.BaseExceptions;

namespace Sentiment.Exceptions;

public class SentimentIdException : DomainException
{
    public SentimentIdException(Guid id)
        : base($"sentiment_id: '{id}' is invalid.")
    {
    }
}