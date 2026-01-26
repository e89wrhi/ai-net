using Resume.Exceptions;

namespace Resume.ValueObjects;

public record SourceFile
{
    public string Value { get; }

    private SourceFile(string value)
    {
        Value = value;
    }

    public static SourceFile Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SourceFileException(value);
        }

        return new SourceFile(value);
    }

    public static implicit operator string(SourceFile @value)
    {
        return @value.Value;
    }
}