using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record SessionId
{
    public Guid Value { get; }

    private SessionId(Guid value)
    {
        Value = value;
    }

    public static SessionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SessionIdException(value);
        }

        return new SessionId(value);
    }

    public static implicit operator Guid(SessionId id)
    {
        return id.Value;
    }
}