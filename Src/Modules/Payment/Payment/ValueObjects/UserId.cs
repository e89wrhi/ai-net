

using Payment.Exceptions;

namespace Payment.ValueObjects;

public record UserId
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UserIdException(value);
        }

        return new UserId(value);
    }

    public static implicit operator Guid(UserId id)
    {
        return id.Value;
    }
}