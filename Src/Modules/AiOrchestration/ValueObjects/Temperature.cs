using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record Temperature
{
    public int Value { get; }

    private Temperature(int value)
    {
        Value = value;
    }

    public static Temperature Of(int value)
    {
        if (value < 0)
        {
            throw new TemperatureException(value);
        }

        return new Temperature(value);
    }

    public static implicit operator int(Temperature id)
    {
        return id.Value;
    }
}