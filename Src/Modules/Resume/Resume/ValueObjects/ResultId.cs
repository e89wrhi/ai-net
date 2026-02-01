using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ResultId
{
    public Guid Value { get; }

    private ResultId(Guid value)
    {
        Value = value;
    }

    public static ResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new ResultId(value);
    }

    public static implicit operator Guid(ResultId id)
    {
        return id.Value;
    }
}