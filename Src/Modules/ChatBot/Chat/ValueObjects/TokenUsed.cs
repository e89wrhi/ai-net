using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record TokenUsed
{
    public int Value { get; }

    private TokenUsed(int value)
    {
        Value = value;
    }

    public static TokenUsed Of(int value)
    {
        if (value < 0)
        {
            throw new TokenUsedException(value);
        }

        return new TokenUsed(value);
    }

    public static implicit operator int(TokenUsed @value)
    {
        return @value.Value;
    }
}