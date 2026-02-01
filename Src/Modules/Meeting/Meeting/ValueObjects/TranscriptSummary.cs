using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record TranscriptSummary
{
    public string Value { get; }

    private TranscriptSummary(string value)
    {
        Value = value;
    }

    public static TranscriptSummary Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SummaryException(value);
        }

        return new TranscriptSummary(value);
    }

    public static implicit operator string(TranscriptSummary @value)
    {
        return @value.Value;
    }
}