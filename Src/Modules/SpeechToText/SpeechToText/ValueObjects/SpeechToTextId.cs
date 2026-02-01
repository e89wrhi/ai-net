using SpeechToText.Exceptions;

namespace SpeechToText.ValueObjects;

public record SpeechToTextId
{
    public Guid Value { get; }

    private SpeechToTextId(Guid value)
    {
        Value = value;
    }

    public static SpeechToTextId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SpeechToTextIdException(value);
        }

        return new SpeechToTextId(value);
    }

    public static implicit operator Guid(SpeechToTextId id)
    {
        return id.Value;
    }
}