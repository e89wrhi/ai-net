using Resume.Exceptions;

namespace Resume.ValueObjects;

public record CandidateName
{
    public string Value { get; }

    private CandidateName(string value)
    {
        Value = value;
    }

    public static CandidateName Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new CandidateNameException(value);
        }

        return new CandidateName(value);
    }

    public static implicit operator string(CandidateName @value)
    {
        return @value.Value;
    }
}