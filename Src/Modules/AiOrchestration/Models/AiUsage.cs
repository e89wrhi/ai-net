using AI.Common.Core;
using AiOrchestration.ValueObjects;

namespace AiOrchestration.Models;

public record AiUsageId
{
    public Guid Value { get; }
    private AiUsageId(Guid value) => Value = value;
    public static AiUsageId Of(Guid value) => new AiUsageId(value);
    public static implicit operator Guid(AiUsageId id) => id.Value;
}

public record AiUsage : Aggregate<AiUsageId>
{
    public Guid UserId { get; private set; }
    public ModelId ModelId { get; private set; } = default!;
    public int TokensConsumed { get; private set; }
    public decimal Cost { get; private set; }
    public string? ProviderName { get; private set; }
    public Guid? ApiKeyId { get; private set; }

    private AiUsage() { }

    public static AiUsage Create(Guid userId, ModelId modelId, int tokens, decimal cost, string? provider = null, Guid? apiKeyId = null)
    {
        return new AiUsage
        {
            Id = AiUsageId.Of(Guid.NewGuid()),
            UserId = userId,
            ModelId = modelId,
            TokensConsumed = tokens,
            Cost = cost,
            ProviderName = provider,
            ApiKeyId = apiKeyId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
