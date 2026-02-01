using User.Exceptions;

namespace User.ValueObjects;

public record ModuleAnalyticsId
{
    public Guid Value { get; }

    private ModuleAnalyticsId(Guid value)
    {
        Value = value;
    }

    public static ModuleAnalyticsId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new UsageModuleIdException(value);
        }

        return new ModuleAnalyticsId(value);
    }

    public static implicit operator Guid(ModuleAnalyticsId id)
    {
        return id.Value;
    }
}