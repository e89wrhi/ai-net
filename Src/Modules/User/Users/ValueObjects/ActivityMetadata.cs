using User.Exceptions;

namespace User.ValueObjects;

public record ActivityMetadata
{
    public string Value { get; }

    private ActivityMetadata(string value)
    {
        Value = value;
    }

    public static ActivityMetadata Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new MetadataException(value);
        }

        return new ActivityMetadata(value);
    }

    public static implicit operator string(ActivityMetadata @value)
    {
        return @value.Value;
    }
}