using Sentiment.Exceptions;

namespace Sentiment.ValueObjects;

public record SentimentText
{
    public string Value { get; }

    private SentimentText(string value)
    {
        Value = value;
    }

    public static SentimentText Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TextException(value);
        }

        return new SentimentText(value);
    }

    public static implicit operator string(SentimentText id)
    {
        return id.Value;
    }
}