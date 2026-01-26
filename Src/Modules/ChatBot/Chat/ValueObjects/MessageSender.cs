using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record MessageSender
{
    public string Value { get; }

    private MessageSender(string value)
    {
        Value = value;
    }

    public static MessageSender Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new MessageSenderException(value);
        }

        return new MessageSender(value);
    }

    public static implicit operator string(MessageSender @value)
    {
        return @value.Value;
    }
}