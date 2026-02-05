using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;

namespace AiOrchestration.Services;

public class AiOrchestrator : IAiOrchestrator
{
    private readonly IAiModelService _modelService;
    private readonly IChatClient _defaultChatClient;

    public AiOrchestrator(IAiModelService modelService, IChatClient defaultChatClient)
    {
        _modelService = modelService;
        _defaultChatClient = defaultChatClient;
    }

    public ChatClientMetadata Metadata => _defaultChatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata ?? new ChatClientMetadata("Default", null);

    public async Task<IChatClient> GetClientAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default)
    {
        var model = await SelectModelAsync(criteria, cancellationToken);
        
        // In a more complex setup, we would return a specific client for the model.
        // For now, we return the injected client, but we can wrap it with metadata.
        return new ChatClientMetadataWrapper(_defaultChatClient, model.Id.Value);
    }

    public async Task<AiModel> SelectModelAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default)
    {
        criteria ??= ModelCriteria.Default;
        var models = await _modelService.GetActiveModelsAsync(cancellationToken);

        // Simple selection logic:
        // 1. Filter by MaxCost if specified
        // 2. Select the first that matches or the first available
        
        var selected = models
            .Where(m => string.IsNullOrEmpty(criteria.ModelId) || m.Id.Value == criteria.ModelId)
            .Where(m => !criteria.MaxCostPerToken.HasValue || _modelService.GetCostPerToken(m.Id) <= criteria.MaxCostPerToken.Value)
            .OrderBy(m => _modelService.GetCostPerToken(m.Id)) // Prefer cheaper models by default
            .FirstOrDefault();

        if (selected == null)
        {
            // Fallback to the first active model
            selected = models.FirstOrDefault() 
                ?? throw new InvalidOperationException("No active AI models available.");
        }

        return selected;
    }
}

// Simple wrapper to attach model ID to metadata
internal class ChatClientMetadataWrapper : IChatClient
{
    private readonly IChatClient _innerClient;
    private readonly ChatClientMetadata _metadata;

    public ChatClientMetadataWrapper(IChatClient innerClient, string modelId)
    {
        _innerClient = innerClient;
        var innerMetadata = innerClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        _metadata = new ChatClientMetadata(
            innerMetadata?.ProviderName ?? "UnknownProvider",
            null, // Uri can be null
            modelId);
    }

    public ChatClientMetadata Metadata => _metadata;

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        => _innerClient.GetResponseAsync(chatMessages, options, cancellationToken);

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        => _innerClient.GetStreamingResponseAsync(chatMessages, options, cancellationToken);

    public object? GetService(Type serviceType, object? key = null) =>
        key is not null ? null :
        serviceType == typeof(ChatClientMetadata) ? _metadata :
        serviceType?.IsInstanceOfType(this) is true ? this :
        _innerClient.GetService(serviceType, key);

    public void Dispose() => _innerClient.Dispose();
}



