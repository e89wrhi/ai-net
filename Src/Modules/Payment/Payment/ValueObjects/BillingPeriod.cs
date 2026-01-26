using Payment.Exceptions;

namespace Payment.ValueObjects;

public record BillingPeriod
{
    public DateTime Value { get; }

    private BillingPeriod(DateTime value)
    {
        Value = value;
    }

    public static BillingPeriod Of(DateTime value)
    {
        if (value < DateTime.UtcNow)
        {
            throw new BillingPeriodException(value);
        }

        return new BillingPeriod(value);
    }

    public static implicit operator DateTime(BillingPeriod @value)
    {
        return @value.Value;
    }
}