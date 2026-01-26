using Payment.Exceptions;

namespace Payment.ValueObjects;

public record PlanLimits
{
    public int Value { get; }

    private PlanLimits(int value)
    {
        Value = value;
    }

    public static PlanLimits Of(int value)
    {
        if (value < 0)
        {
            throw new PlanLimitException(value);
        }

        return new PlanLimits(value);
    }

    public static implicit operator int(PlanLimits @value)
    {
        return @value.Value;
    }
}