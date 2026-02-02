using AiOrchestration.ValueObjects;

namespace AiOrchestration.Services;

public interface IAiModelService
{
    decimal GetCostPerToken(ModelId modelId);
    string GetProviderName(ModelId modelId);
}

public class AiModelService : IAiModelService
{
    // A simple registry for now. In a real system, this would come from a database.
    private static readonly Dictionary<string, decimal> _costs = new()
    {
        { "gpt-4o", 0.000005m },
        { "gpt-4o-mini", 0.00000015m },
        { "gpt-4", 0.00003m },
        { "gpt-3.5-turbo", 0.0000015m },
        { "ollama-llama3", 0m }, // Local model
        { "default-model", 0.000002m },
        { "Simulated", 0m }
    };

    public decimal GetCostPerToken(ModelId modelId)
    {
        return _costs.TryGetValue(modelId.Value, out var cost) ? cost : 0.000002m;
    }

    public string GetProviderName(ModelId modelId)
    {
        if (modelId.Value.Contains("gpt")) return "OpenAI";
        if (modelId.Value.Contains("ollama")) return "Ollama";
        return "Generic";
    }
}
