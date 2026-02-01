using Sentiment.Exceptions;

namespace Sentiment.ValueObjects;

public record SentimentResultId
{
    public Guid Value { get; }

    private SentimentResultId(Guid value)
    {
        Value = value;
    }

    public static SentimentResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new SentimentResultId(value);
    }

    public static implicit operator Guid(SentimentResultId id)
    {
        return id.Value;
    }
}