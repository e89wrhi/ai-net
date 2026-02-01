using TextToSpeech.Exceptions;

namespace TextToSpeech.ValueObjects;

public record SynthesizedSpeech
{
    public string Value { get; }

    private SynthesizedSpeech(string value)
    {
        Value = value;
    }

    public static SynthesizedSpeech Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SythesizedSpeechException(value);
        }

        return new SynthesizedSpeech(value);
    }

    public static implicit operator string(SynthesizedSpeech @value)
    {
        return @value.Value;
    }
}