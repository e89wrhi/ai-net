using ImageEdit.Exceptions;

namespace ImageEdit.ValueObjects;

public record ImageEditResultId
{
    public Guid Value { get; }

    private ImageEditResultId(Guid value)
    {
        Value = value;
    }

    public static ImageEditResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ImageEditIdException(value);
        }

        return new ImageEditResultId(value);
    }

    public static implicit operator Guid(ImageEditResultId id)
    {
        return id.Value;
    }
}