using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record ImageCaptionId
{
    public Guid Value { get; }

    private ImageCaptionId(Guid value)
    {
        Value = value;
    }

    public static ImageCaptionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ImageIdException(value);
        }

        return new ImageCaptionId(value);
    }

    public static implicit operator Guid(ImageCaptionId id)
    {
        return id.Value;
    }
}