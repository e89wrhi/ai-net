

using Payment.Exceptions;

namespace Payment.ValueObjects;

public record SubscriptionId
{
    public Guid Value { get; }

    private SubscriptionId(Guid value)
    {
        Value = value;
    }

    public static SubscriptionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SubscriptionIdException(value);
        }

        return new SubscriptionId(value);
    }

    public static implicit operator Guid(SubscriptionId id)
    {
        return id.Value;
    }
}