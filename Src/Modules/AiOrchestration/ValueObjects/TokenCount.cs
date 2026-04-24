using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record TokenCount
{
    public long Value { get; }

    private TokenCount(long value)
    {
        Value = value;
    }

    public static TokenCount Of(long value)
    {
        if (value < 0)
        {
            throw new TokenCountException(value);
        }

        return new TokenCount(value);
    }

    public static implicit operator long(TokenCount? tokenCount)
    {
        return tokenCount?.Value ?? 0;
    }
}