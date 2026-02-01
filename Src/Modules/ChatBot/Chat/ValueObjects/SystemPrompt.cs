using ChatBot.Exceptions;

namespace ChatBot.ValueObjects;

public record SystemPrompt
{
    public string Value { get; }

    private SystemPrompt(string value)
    {
        Value = value;
    }

    public static SystemPrompt Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SystemPromptException(value);
        }

        return new SystemPrompt(value);
    }

    public static implicit operator string(SystemPrompt @value)
    {
        return @value.Value;
    }
}