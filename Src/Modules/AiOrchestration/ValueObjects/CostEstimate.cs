using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record CostEstimate
{
    public decimal Value { get; }

    private CostEstimate(decimal value)
    {
        Value = value;
    }

    public static CostEstimate Of(decimal value)
    {
        if (value < 0)
        {
            throw new CostEstimateException(value);
        }

        return new CostEstimate(value);
    }

    public static implicit operator decimal(CostEstimate id)
    {
        return id.Value;
    }
}