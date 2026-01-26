using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record MessageTime
{
    public DateTime Value { get; }

    private MessageTime(DateTime value)
    {
        Value = value;
    }

    public static MessageTime Of(DateTime value)
    {
        if (value > DateTime.UtcNow)
        {
            throw new MessageTimeException(value);
        }

        return new MessageTime(value);
    }

    public static implicit operator DateTime(MessageTime @value)
    {
        return @value.Value;
    }
}