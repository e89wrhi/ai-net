

using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ExpirenceId
{
    public Guid Value { get; }

    private ExpirenceId(Guid value)
    {
        Value = value;
    }

    public static ExpirenceId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ExpirenceIdException(value);
        }

        return new ExpirenceId(value);
    }

    public static implicit operator Guid(ExpirenceId id)
    {
        return id.Value;
    }
}