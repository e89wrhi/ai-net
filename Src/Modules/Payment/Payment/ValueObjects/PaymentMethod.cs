using Payment.Exceptions;

namespace Payment.ValueObjects;

public record PaymentMethod
{
    public string Provider { get; }
    public string AccountNumber { get; }

    public PaymentMethod(string provider, string accountNumber)
    {
        Provider = provider;
        AccountNumber = accountNumber;
    }
}
