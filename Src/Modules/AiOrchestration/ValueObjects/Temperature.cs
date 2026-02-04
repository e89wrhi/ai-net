using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record Temperature
{
    public float Value { get; }

    private Temperature(float value)
    {
        Value = value;
    }

    public static Temperature Of(float value)
    {
        if (value < 0)
        {
            throw new TemperatureException(value);
        }

        return new Temperature(value);
    }

    public static implicit operator float(Temperature id)
    {
        return id.Value;
    }
}