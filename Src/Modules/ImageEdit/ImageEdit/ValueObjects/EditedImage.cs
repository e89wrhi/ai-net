using ImageEdit.Exceptions;

namespace ImageEdit.ValueObjects;

public record EditedImage
{
    public string Value { get; }

    private EditedImage(string value)
    {
        Value = value;
    }

    public static EditedImage Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new EditedImageException(value);
        }

        return new EditedImage(value);
    }

    public static implicit operator string(EditedImage @value)
    {
        return @value.Value;
    }
}