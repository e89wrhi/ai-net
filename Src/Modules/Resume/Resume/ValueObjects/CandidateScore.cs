using Resume.Exceptions;

namespace Resume.ValueObjects;

public record CandidateScore
{
    public double Value { get; }

    private CandidateScore(double value)
    {
        Value = value;
    }

    public static CandidateScore Of(double value)
    {
        if (value < 0)
        {
            throw new CandidateScoreException(value);
        }

        return new CandidateScore(value);
    }

    public static implicit operator double(CandidateScore @value)
    {
        return @value.Value;
    }
}