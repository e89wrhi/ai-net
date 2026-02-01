using TextToSpeech.Exceptions;

namespace TextToSpeech.ValueObjects;

public record TextToSpeechId
{
    public Guid Value { get; }

    private TextToSpeechId(Guid value)
    {
        Value = value;
    }

    public static TextToSpeechId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new TextToSpeechIdException(value);
        }

        return new TextToSpeechId(value);
    }

    public static implicit operator Guid(TextToSpeechId id)
    {
        return id.Value;
    }
}