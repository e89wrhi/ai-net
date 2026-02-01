using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record DebugSummary
{
    public string Value { get; }

    private DebugSummary(string value)
    {
        Value = value;
    }

    public static DebugSummary Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new CodeDebugSummaryException(value);
        }

        return new DebugSummary(value);
    }

    public static implicit operator string(DebugSummary @value)
    {
        return @value.Value;
    }
}