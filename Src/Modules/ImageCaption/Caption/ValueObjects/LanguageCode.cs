using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record LanguageCode
{
    public string Value { get; }

    private LanguageCode(string value)
    {
        Value = value;
    }

    public static LanguageCode Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new LanguageCodeException(value);
        }

        return new LanguageCode(value);
    }

    public static implicit operator string(LanguageCode id)
    {
        return id.Value;
    }
}