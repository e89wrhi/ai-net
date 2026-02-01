using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record MeetingId
{
    public Guid Value { get; }

    private MeetingId(Guid value)
    {
        Value = value;
    }

    public static MeetingId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new MeetingIdException(value);
        }

        return new MeetingId(value);
    }

    public static implicit operator Guid(MeetingId id)
    {
        return id.Value;
    }
}