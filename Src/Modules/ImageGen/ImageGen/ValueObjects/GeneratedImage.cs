using ImageGen.Exceptions;

namespace ImageGen.ValueObjects;

public record GeneratedImage
{
    public string Value { get; }

    private GeneratedImage(string value)
    {
        Value = value;
    }

    public static GeneratedImage Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new GeneratedImageException(value);
        }

        return new GeneratedImage(value);
    }

    public static implicit operator string(GeneratedImage @value)
    {
        return @value.Value;
    }
}