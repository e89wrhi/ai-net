using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record AudioSource
{
    public string Value { get; }

    private AudioSource(string value)
    {
        Value = value;
    }

    public static AudioSource Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new AudioSourceException(value);
        }

        return new AudioSource(value);
    }

    public static implicit operator string(AudioSource @value)
    {
        return @value.Value;
    }
}