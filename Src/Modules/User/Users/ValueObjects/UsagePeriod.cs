using User.Exceptions;

namespace User.ValueObjects;

public record UsagePeriod
{
    public string Value { get; }

    private UsagePeriod(string value)
    {
        Value = value;
    }

    public static UsagePeriod Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new UsagePeriodException(value);
        }

        return new UsagePeriod(value);
    }

    public static implicit operator string(UsagePeriod @value)
    {
        return @value.Value;
    }
}