

using User.Exceptions;

namespace User.ValueObjects;

public record UsageContainerId
{
    public Guid Value { get; }

    private UsageContainerId(Guid value)
    {
        Value = value;
    }

    public static UsageContainerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UsageContainerIdException(value);
        }

        return new UsageContainerId(value);
    }

    public static implicit operator Guid(UsageContainerId id)
    {
        return id.Value;
    }
}