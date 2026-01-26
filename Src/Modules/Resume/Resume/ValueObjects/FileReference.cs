using Resume.Exceptions;

namespace Resume.ValueObjects;

public record FileReference
{
    public string Value { get; }

    private FileReference(string value)
    {
        Value = value;
    }

    public static FileReference Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SourceFileException(value);
        }

        return new FileReference(value);
    }

    public static implicit operator string(FileReference @value)
    {
        return @value.Value;
    }
}