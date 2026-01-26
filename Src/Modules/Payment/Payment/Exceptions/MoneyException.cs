using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class MoneyException : DomainException
{
    public MoneyException(decimal money)
        : base($"money: '{money}' is invalid.")
    {
    }
}
