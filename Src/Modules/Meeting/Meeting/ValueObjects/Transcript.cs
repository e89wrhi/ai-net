using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record Transcript
{
    public string Value { get; }

    private Transcript(string value)
    {
        Value = value;
    }

    public static Transcript Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TranscriptException(value);
        }

        return new Transcript(value);
    }

    public static implicit operator string(Transcript @value)
    {
        return @value.Value;
    }
}