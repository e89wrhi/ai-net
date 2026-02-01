using AutoComplete.Exceptions;

namespace AutoComplete.ValueObjects;

public record AutoCompleteSuggestion
{
    public string Value { get; }

    public AutoCompleteSuggestion(string value)
    {
        Value = value ?? string.Empty;
    }


    public static AutoCompleteSuggestion Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new AutoCompleteSuggestionException(value);
        }

        return new AutoCompleteSuggestion(value);
    }

    public static implicit operator string(AutoCompleteSuggestion id)
    {
        return id.Value;
    }
}
