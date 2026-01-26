using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record Title
{
    public string Value { get; }

    private Title(string value)
    {
        Value = value;
    }

    public static Title Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TitleException(value);
        }

        return new Title(value);
    }

    public static implicit operator string(Title @value)
    {
        return @value.Value;
    }
}