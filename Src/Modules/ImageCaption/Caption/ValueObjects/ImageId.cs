

using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record ImageId
{
    public Guid Value { get; }

    private ImageId(Guid value)
    {
        Value = value;
    }

    public static ImageId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ImageIdException(value);
        }

        return new ImageId(value);
    }

    public static implicit operator Guid(ImageId id)
    {
        return id.Value;
    }
}