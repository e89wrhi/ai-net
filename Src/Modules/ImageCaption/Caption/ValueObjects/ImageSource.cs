using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public class ImageSource
{
    public string Value { get; }

    private ImageSource(string value)
    {
        Value = value;
    }

    public static ImageSource Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ImageSourceException(value);
        }

        return new ImageSource(value);
    }

    public static implicit operator string(ImageSource id)
    {
        return id.Value;
    }
}