

using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record ActionItemId
{
    public Guid Value { get; }

    private ActionItemId(Guid value)
    {
        Value = value;
    }

    public static ActionItemId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ActionIdException(value);
        }

        return new ActionItemId(value);
    }

    public static implicit operator Guid(ActionItemId id)
    {
        return id.Value;
    }
}