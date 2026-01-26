using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

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
            throw new FileReferenceException(value);
        }

        return new FileReference(value);
    }

    public static implicit operator string(FileReference @value)
    {
        return @value.Value;
    }
}