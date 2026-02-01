using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record TranscriptId
{
    public Guid Value { get; }

    private TranscriptId(Guid value)
    {
        Value = value;
    }

    public static TranscriptId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new TranscriptIdException(value);
        }

        return new TranscriptId(value);
    }

    public static implicit operator Guid(TranscriptId id)
    {
        return id.Value;
    }
}