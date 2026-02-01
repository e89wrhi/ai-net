using TextToSpeech.Exceptions;

namespace TextToSpeech.ValueObjects;

public record TextToSpeechResultId
{
    public Guid Value { get; }

    private TextToSpeechResultId(Guid value)
    {
        Value = value;
    }

    public static TextToSpeechResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new TextToSpeechResultId(value);
    }

    public static implicit operator Guid(TextToSpeechResultId id)
    {
        return id.Value;
    }
}