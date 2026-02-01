using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record CaptionConfidence
{
    public double Value { get; }

    private CaptionConfidence(double value)
    {
        Value = value;
    }

    public static CaptionConfidence Of(double value)
    {
        if (value < 0)
        {
            throw new ConfidenceScoreException(value);
        }

        return new CaptionConfidence(value);
    }

    public static implicit operator double(CaptionConfidence @value)
    {
        return @value.Value;
    }
}