using Payment.Exceptions;

namespace Payment.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new MoneyException(amount);
        }

        Amount = amount;
        Currency = currency;
    }

    public static Money Of(decimal amount, string currency = "USD")
    {
        return new Money(amount, currency);
    }

    public static implicit operator decimal(Money @value)
    {
        return @value.Amount;
    }
}