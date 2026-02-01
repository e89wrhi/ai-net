using Translate.Exceptions;

namespace Translate.ValueObjects;

public record TranslatedText
{
    public string Value { get; }

    private TranslatedText(string value)
    {
        Value = value;
    }

    public static TranslatedText Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TextException(value);
        }

        return new TranslatedText(value);
    }

    public static implicit operator string(TranslatedText @value)
    {
        return @value.Value;
    }
}