using User.Exceptions;

namespace User.ValueObjects;

public record UserActivityId
{
    public Guid Value { get; }

    private UserActivityId(Guid value)
    {
        Value = value;
    }

    public static UserActivityId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UserActivityIdException(value);
        }

        return new UserActivityId(value);
    }

    public static implicit operator Guid(UserActivityId id)
    {
        return id.Value;
    }
}