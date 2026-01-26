

using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record ParticipantId
{
    public Guid Value { get; }

    private ParticipantId(Guid value)
    {
        Value = value;
    }

    public static ParticipantId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ParticipantIdException(value);
        }

        return new ParticipantId(value);
    }

    public static implicit operator Guid(ParticipantId id)
    {
        return id.Value;
    }
}