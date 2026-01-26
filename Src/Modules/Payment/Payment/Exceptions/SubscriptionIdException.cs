using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class SubscriptionIdException : DomainException
{
    public SubscriptionIdException(Guid subscription_id)
        : base($"subscription_id: '{subscription_id}' is invalid.")
    {
    }
}
