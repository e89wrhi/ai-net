using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record ModelId
{
    public Guid Value { get; }

    private ModelId(Guid value)
    {
        Value = value;
    }

    public static ModelId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ModelIdException(value);
        }

        return new ModelId(value);
    }

    public static implicit operator Guid(ModelId id)
    {
        return id.Value;
    }
}