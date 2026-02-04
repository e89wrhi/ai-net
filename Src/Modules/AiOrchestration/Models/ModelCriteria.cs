using AiOrchestration.ValueObjects;

namespace AiOrchestration.Models;

public record ModelCriteria
{
    public string? ModelId { get; init; }
    public string? PreferredCapability { get; init; }
    public decimal? MaxCostPerToken { get; init; }
    public TimeSpan? MaxLatency { get; init; }
    public bool MustBeActive { get; init; } = true;

    public static ModelCriteria Default => new();
}
