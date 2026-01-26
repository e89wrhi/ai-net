using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record TokenUsed
{
    public string Value { get; }

    private TokenUsed(string value)
    {
        Value = value;
    }

    public static TokenUsed Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TokenUsedException(value);
        }

        return new TokenUsed(value);
    }

    public static implicit operator string(TokenUsed @value)
    {
        return @value.Value;
    }
}