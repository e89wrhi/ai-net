using Payment.Exceptions;

namespace Payment.ValueObjects;

public record Money
{
    public decimal Value { get; }

    private Money(decimal value)
    {
        Value = value;
    }

    public static Money Of(decimal value)
    {
        if (value < 0)
        {
            throw new MoneyException(value);
        }

        return new Money(value);
    }

    public static implicit operator decimal(Money @value)
    {
        return @value.Value;
    }
}