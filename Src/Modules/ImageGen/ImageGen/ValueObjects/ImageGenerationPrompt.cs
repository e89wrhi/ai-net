using ImageGen.Exceptions;

namespace ImageGen.ValueObjects;

public record ImageGenerationPrompt
{
    public string Value { get; }

    private ImageGenerationPrompt(string value)
    {
        Value = value;
    }

    public static ImageGenerationPrompt Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new PromptException(value);
        }

        return new ImageGenerationPrompt(value);
    }

    public static implicit operator string(ImageGenerationPrompt @value)
    {
        return @value.Value;
    }
}