using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ResumeSummary
{
    public string Value { get; }

    private ResumeSummary(string value)
    {
        Value = value;
    }

    public static ResumeSummary Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SummaryException(value);
        }

        return new ResumeSummary(value);
    }

    public static implicit operator string(ResumeSummary @value)
    {
        return @value.Value;
    }
}