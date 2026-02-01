using Summary.Exceptions;

namespace Summary.ValueObjects;

public record SummaryText
{
    public string Value { get; }

    private SummaryText(string value)
    {
        Value = value;
    }

    public static SummaryText Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TextException(value);
        }

        return new SummaryText(value);
    }

    public static implicit operator string(SummaryText @value)
    {
        return @value.Value;
    }
}