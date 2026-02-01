using User.Exceptions;

namespace User.ValueObjects;

public record UserActionId
{
    public Guid Value { get; }

    private UserActionId(Guid value)
    {
        Value = value;
    }

    public static UserActionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UserActionIdException(value);
        }

        return new UserActionId(value);
    }

    public static implicit operator Guid(UserActionId id)
    {
        return id.Value;
    }
}