using Sentiment.Exceptions;

namespace Sentiment.ValueObjects;

public record SentimentScore
{
    public double Value { get; }

    private SentimentScore(double value)
    {
        Value = value;
    }

    public static SentimentScore Of(double value)
    {
        if (value < 0)
        {
            throw new ScoreException(value);
        }

        return new SentimentScore(value);
    }

    public static implicit operator double(SentimentScore @value)
    {
        return @value.Value;
    }
}