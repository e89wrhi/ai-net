using SpeechToText.Exceptions;

namespace SpeechToText.ValueObjects;

public record SpeechToTextResultId
{
    public Guid Value { get; }

    private SpeechToTextResultId(Guid value)
    {
        Value = value;
    }

    public static SpeechToTextResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new SpeechToTextResultId(value);
    }

    public static implicit operator Guid(SpeechToTextResultId id)
    {
        return id.Value;
    }
}