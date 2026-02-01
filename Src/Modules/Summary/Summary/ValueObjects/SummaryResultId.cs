using Summary.Exceptions;

namespace Summary.ValueObjects;

public record SummaryResultId
{
    public Guid Value { get; }

    private SummaryResultId(Guid value)
    {
        Value = value;
    }

    public static SummaryResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new SummaryResultId(value);
    }

    public static implicit operator Guid(SummaryResultId id)
    {
        return id.Value;
    }
}