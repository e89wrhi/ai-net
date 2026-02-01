using AiOrchestration.Exceptions;
using MassTransit;

namespace AiOrchestration.ValueObjects;

public record Provider
{
    public string Value { get; }

    private Provider(string value)
    {
        Value = value;
    }

    public static Provider Of(string value)
    {
        if (value == string.Empty)
        {
            throw new ProviderException(value);
        }

        return new Provider(value);
    }

    public static implicit operator string(Provider id)
    {
        return id.Value;
    }
}