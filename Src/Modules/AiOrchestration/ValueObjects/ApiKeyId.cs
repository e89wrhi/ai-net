namespace AiOrchestration.ValueObjects;

public record ApiKeyId
{
    public Guid Value { get; }

    private ApiKeyId(Guid value)
    {
        Value = value;
    }

    public static ApiKeyId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("ApiKeyId cannot be empty", nameof(value));
        }

        return new ApiKeyId(value);
    }

    public static implicit operator Guid(ApiKeyId id)
    {
        return id.Value;
    }
}
