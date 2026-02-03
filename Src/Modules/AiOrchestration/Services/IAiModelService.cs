using AiOrchestration.ValueObjects;
using AiOrchestration.Models;

namespace AiOrchestration.Services;

public interface IAiModelService
{
    Task<IEnumerable<AiModel>> GetActiveModelsAsync(CancellationToken cancellationToken = default);
    Task<AiModel?> GetModelAsync(ModelId modelId, CancellationToken cancellationToken = default);
    decimal GetCostPerToken(ModelId modelId);
    string GetProviderName(ModelId modelId);
}

public class AiModelService : IAiModelService
{
    // Mock data for models. In a real system, these would be in a database.
    private static readonly List<AiModel> _models = new()
    {
        AiModel.Create(ModelId.Of("gpt-4o"), "GPT-4o", "Omni model", "v1", Provider.Of("OpenAI")),
        AiModel.Create(ModelId.Of("gpt-4o-mini"), "GPT-4o Mini", "Small & Fast", "v1", Provider.Of("OpenAI")),
        AiModel.Create(ModelId.Of("ollama-llama3"), "Llama 3", "Local Llama 3", "v1", Provider.Of("Ollama")),
        AiModel.Create(ModelId.Of("Simulated"), "Simulated Model", "For testing", "v1", Provider.Of("Generic"))
    };

    private static readonly Dictionary<string, decimal> _costs = new()
    {
        { "gpt-4o", 0.000005m },
        { "gpt-4o-mini", 0.00000015m },
        { "gpt-4", 0.00003m },
        { "gpt-3.5-turbo", 0.0000015m },
        { "ollama-llama3", 0m },
        { "default-model", 0.000002m },
        { "Simulated", 0m }
    };

    public Task<IEnumerable<AiModel>> GetActiveModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_models.Where(m => m.IsActive));
    }

    public Task<AiModel?> GetModelAsync(ModelId modelId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_models.FirstOrDefault(m => m.Id == modelId));
    }

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

