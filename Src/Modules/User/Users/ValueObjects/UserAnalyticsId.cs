using User.Exceptions;

namespace User.ValueObjects;

public record UserAnalyticsId
{
    public Guid Value { get; }

    private UserAnalyticsId(Guid value)
    {
        Value = value;
    }

    public static UserAnalyticsId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UserAnalyticsIdException(value);
        }

        return new UserAnalyticsId(value);
    }

    public static implicit operator Guid(UserAnalyticsId id)
    {
        return id.Value;
    }
}