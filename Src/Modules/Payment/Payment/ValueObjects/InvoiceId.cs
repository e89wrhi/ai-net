using Payment.Exceptions;

namespace Payment.ValueObjects;

public record InvoiceId
{
    public Guid Value { get; }

    private InvoiceId(Guid value)
    {
        Value = value;
    }

    public static InvoiceId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvoiceIdException(value);
        }

        return new InvoiceId(value);
    }

    public static implicit operator Guid(InvoiceId id)
    {
        return id.Value;
    }
}