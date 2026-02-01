using User.Exceptions;

namespace User.ValueObjects;

public record ModuleId
{
    public Guid Value { get; }

    private ModuleId(Guid value)
    {
        Value = value;
    }

    public static ModuleId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ModuleIdException(value);
        }

        return new ModuleId(value);
    }

    public static implicit operator Guid(ModuleId id)
    {
        return id.Value;
    }
}