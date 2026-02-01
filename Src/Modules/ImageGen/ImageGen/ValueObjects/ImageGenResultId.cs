using ImageGen.Exceptions;

namespace ImageGen.ValueObjects;

public record ImageGenResultId
{
    public Guid Value { get; }

    private ImageGenResultId(Guid value)
    {
        Value = value;
    }

    public static ImageGenResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new ImageGenResultId(value);
    }

    public static implicit operator Guid(ImageGenResultId id)
    {
        return id.Value;
    }
}