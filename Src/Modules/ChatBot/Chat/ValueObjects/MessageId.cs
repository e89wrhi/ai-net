using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record MessageId
{
    public Guid Value { get; }

    private MessageId(Guid value)
    {
        Value = value;
    }

    public static MessageId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new MessageIdException(value);
        }

        return new MessageId(value);
    }

    public static implicit operator Guid(MessageId id)
    {
        return id.Value;
    }
}