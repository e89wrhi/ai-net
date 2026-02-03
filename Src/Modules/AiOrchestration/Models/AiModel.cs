using AI.Common.Core;
using AiOrchestration.ValueObjects;

public record AiModel : Aggregate<ModelId>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string AiVersion { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public Provider Provider { get; private set; } = default!;
    public string Capabilities { get; private set; } = default!;
    public CostEstimate CostEstimate { get; private set; } = default!;
    public TokenCount TokenCount { get; private set; } = default!;
    public LatencyBudget LatencyBudget { get; private set; } = default!;
    public CostEstimate CostPerToken { get; private set; } = default!;
    public TokenCount MaxTokens { get; private set; } = default!;

    private AiModel() { }

    public static AiModel Create(ModelId id, string name, string description, string version, Provider provider)
    {
        var model = new AiModel
        {
            Id = id,
            Name = name,
            Description = description,
            AiVersion = version,
            Provider = provider,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        return model;
    }
}