using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record MessageContent
{
    public string Value { get; }

    private MessageContent(string value)
    {
        Value = value;
    }

    public static MessageContent Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new MessageContentException(value);
        }

        return new MessageContent(value);
    }

    public static implicit operator string(MessageContent @value)
    {
        return @value.Value;
    }
}