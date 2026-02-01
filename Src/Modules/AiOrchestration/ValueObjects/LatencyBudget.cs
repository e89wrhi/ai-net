using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record LatencyBudget
{
    public decimal Value { get; }

    private LatencyBudget(decimal value)
    {
        Value = value;
    }

    public static LatencyBudget Of(decimal value)
    {
        if (value < 0)
        {
            throw new LatencyBudgetException(value);
        }

        return new LatencyBudget(value);
    }

    public static implicit operator decimal(LatencyBudget id)
    {
        return id.Value;
    }
}