using Payment.Exceptions;

namespace Payment.ValueObjects;

public record CurrencyCode
{
    public string Value { get; }

    private CurrencyCode(string value)
    {
        Value = value;
    }

    public static CurrencyCode Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new CurrencyCodeException(value);
        }

        return new CurrencyCode(value);
    }

    public static implicit operator string(CurrencyCode @value)
    {
        return @value.Value;
    }
}