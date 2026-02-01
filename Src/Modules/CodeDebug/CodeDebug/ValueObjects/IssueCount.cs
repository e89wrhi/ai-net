using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record IssueCount
{
    public int Value { get; }

    private IssueCount(int value)
    {
        Value = value;
    }

    public static IssueCount Of(int value)
    {
        if (value < 0)
        {
            throw new IssueCountException(value);
        }

        return new IssueCount(value);
    }

    public static implicit operator int(IssueCount @value)
    {
        return @value.Value;
    }
}