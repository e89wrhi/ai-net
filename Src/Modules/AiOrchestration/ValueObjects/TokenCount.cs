using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record TokenCount
{
    public int Value { get; }

    private TokenCount(int value)
    {
        Value = value;
    }

    public static TokenCount Of(int value)
    {
        if (value < 0)
        {
            throw new TokenCountException(value);
        }

        return new TokenCount(value);
    }

    public static implicit operator int(TokenCount id)
    {
        return id.Value;
    }
}