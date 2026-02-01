using AI.Common.BaseExceptions;

namespace Sentiment.Exceptions;

public class SentimentNotFoundException : DomainException
{
    public SentimentNotFoundException(Guid sentimentId)
        : base($"sentiment: '{sentimentId}' not found.")
    {
    }
}

public class SentimentAlreadyExistException : DomainException
{
    public SentimentAlreadyExistException(Guid sentimentId)
        : base($"sentiment: '{sentimentId}' already exist.")
    {
    }
}
