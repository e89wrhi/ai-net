using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record ImageCaptionResultId
{
    public Guid Value { get; }

    private ImageCaptionResultId(Guid value)
    {
        Value = value;
    }

    public static ImageCaptionResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new ImageCaptionResultId(value);
    }

    public static implicit operator Guid(ImageCaptionResultId id)
    {
        return id.Value;
    }
}