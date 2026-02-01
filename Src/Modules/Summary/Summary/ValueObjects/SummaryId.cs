using Summary.Exceptions;

namespace Summary.ValueObjects;

public record SummaryId
{
    public Guid Value { get; }

    private SummaryId(Guid value)
    {
        Value = value;
    }

    public static SummaryId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SummaryIdException(value);
        }

        return new SummaryId(value);
    }

    public static implicit operator Guid(SummaryId id)
    {
        return id.Value;
    }
}