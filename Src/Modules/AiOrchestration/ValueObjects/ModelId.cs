using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record ModelId
{
    public string Value { get; }

    private ModelId(string value)
    {
        Value = value;
    }

    public static ModelId Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ModelIdException(Guid.Empty);
        }

        return new ModelId(value);
    }

    public static ModelId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ModelIdException(value);
        }

        return new ModelId(value.ToString());
    }

    public static implicit operator string(ModelId id)
    {
        return id.Value;
    }
}