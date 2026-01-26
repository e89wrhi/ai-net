using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ParsedText
{
    public string Value { get; }

    private ParsedText(string value)
    {
        Value = value;
    }

    public static ParsedText Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TextException(value);
        }

        return new ParsedText(value);
    }

    public static implicit operator string(ParsedText @value)
    {
        return @value.Value;
    }
}