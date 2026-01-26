using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class SubscriptionException : DomainException
{
    public SubscriptionException(string subscription)
        : base($"subscription: '{subscription}' is invalid.")
    {
    }
}
