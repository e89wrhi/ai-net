using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class CurrencyCodeException : DomainException
{
    public CurrencyCodeException(string currency)
        : base($"currenccy: '{currency}' is invalid.")
    {
    }
}
