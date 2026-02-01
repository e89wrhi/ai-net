using ImageEdit.Exceptions;

namespace ImageEdit.ValueObjects;

public record ImageEditId
{
    public Guid Value { get; }

    private ImageEditId(Guid value)
    {
        Value = value;
    }

    public static ImageEditId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ImageEditIdException(value);
        }

        return new ImageEditId(value);
    }

    public static implicit operator Guid(ImageEditId id)
    {
        return id.Value;
    }
}