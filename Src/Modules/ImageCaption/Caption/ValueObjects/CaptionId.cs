

using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record CaptionId
{
    public Guid Value { get; }

    private CaptionId(Guid value)
    {
        Value = value;
    }

    public static CaptionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new CaptionIdException(value);
        }

        return new CaptionId(value);
    }

    public static implicit operator Guid(CaptionId id)
    {
        return id.Value;
    }
}