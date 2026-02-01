using ImageEdit.Exceptions;

namespace ImageEdit.ValueObjects;

public record ImageEditPrompt
{
    public string Value { get; }

    private ImageEditPrompt(string value)
    {
        Value = value;
    }

    public static ImageEditPrompt Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new PromptException(value);
        }

        return new ImageEditPrompt(value);
    }

    public static implicit operator string(ImageEditPrompt @value)
    {
        return @value.Value;
    }
}