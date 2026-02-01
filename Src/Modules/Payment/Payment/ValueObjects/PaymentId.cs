using Payment.Exceptions;

namespace Payment.ValueObjects;

public record PaymentId
{
    public Guid Value { get; }

    private PaymentId(Guid value)
    {
        Value = value;
    }

    public static PaymentId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new PaymentIdException(value);
        }

        return new PaymentId(value);
    }

    public static implicit operator Guid(PaymentId id)
    {
        return id.Value;
    }
}