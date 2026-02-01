using Payment.Exceptions;

namespace Payment.ValueObjects;

public record UsageChargeId
{
    public Guid Value { get; }

    private UsageChargeId(Guid value)
    {
        Value = value;
    }

    public static UsageChargeId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UsageChargeIdException(value);
        }

        return new UsageChargeId(value);
    }

    public static implicit operator Guid(UsageChargeId id)
    {
        return id.Value;
    }
}