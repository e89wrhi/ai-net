using Sentiment.Exceptions;

namespace Sentiment.ValueObjects;

public record SentimentId
{
    public Guid Value { get; }

    private SentimentId(Guid value)
    {
        Value = value;
    }

    public static SentimentId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SentimentIdException(value);
        }

        return new SentimentId(value);
    }

    public static implicit operator Guid(SentimentId id)
    {
        return id.Value;
    }
}