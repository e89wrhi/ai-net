using AutoComplete.Exceptions;

namespace AutoComplete.ValueObjects;

public record AutoCompletePrompt
{
    public string Value { get; }

    private AutoCompletePrompt(string value)
    {
        Value = value;
    }

    public static AutoCompletePrompt Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new AutoCompletePromptException(value);
        }

        return new AutoCompletePrompt(value);
    }

    public static implicit operator string(AutoCompletePrompt @value)
    {
        return @value.Value;
    }
}