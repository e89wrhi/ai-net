

using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record DetectedObjectId
{
    public Guid Value { get; }

    private DetectedObjectId(Guid value)
    {
        Value = value;
    }

    public static DetectedObjectId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ObjectIdException(value);
        }

        return new DetectedObjectId(value);
    }

    public static implicit operator Guid(DetectedObjectId id)
    {
        return id.Value;
    }
}