using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ConfidenceScore
{
    public double Value { get; }

    private ConfidenceScore(double value)
    {
        Value = value;
    }

    public static ConfidenceScore Of(double value)
    {
        if (value < 0)
        {
            throw new ScoreException(value);
        }

        return new ConfidenceScore(value);
    }

    public static implicit operator double(ConfidenceScore @value)
    {
        return @value.Value;
    }
}