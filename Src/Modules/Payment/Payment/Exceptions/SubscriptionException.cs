using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class SubscriptionNotFoundException : DomainException
{
    public SubscriptionNotFoundException(Guid subscription)
        : base($"subscription: '{subscription}' is invalid.")
    {
    }
}
