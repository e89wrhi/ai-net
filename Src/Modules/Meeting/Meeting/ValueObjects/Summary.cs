using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record Summary
{
    public string Value { get; }

    private Summary(string value)
    {
        Value = value;
    }

    public static Summary Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SummaryException(value);
        }

        return new Summary(value);
    }

    public static implicit operator string(Summary @value)
    {
        return @value.Value;
    }
}