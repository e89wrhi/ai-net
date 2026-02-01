using ImageGen.Exceptions;

namespace ImageGen.ValueObjects;

public record ImageGenId
{
    public Guid Value { get; }

    private ImageGenId(Guid value)
    {
        Value = value;
    }

    public static ImageGenId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ImageGenIdException(value);
        }

        return new ImageGenId(value);
    }

    public static implicit operator Guid(ImageGenId id)
    {
        return id.Value;
    }
}