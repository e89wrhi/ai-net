using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using AI.Common.Web;

namespace AiOrchestration.Services;

public class AiOrchestrator : IAiOrchestrator
{
    private readonly IAiModelService _modelService;
    private readonly IChatClient _defaultChatClient;
    private readonly IApiKeyService _apiKeyService;
    private readonly IUsageService _usageService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AiOrchestrator(
        IAiModelService modelService, 
        IChatClient defaultChatClient,
        IApiKeyService apiKeyService,
        IUsageService usageService,
        ICurrentUserProvider currentUserProvider)
    {
        _modelService = modelService;
        _defaultChatClient = defaultChatClient;
        _apiKeyService = apiKeyService;
        _usageService = usageService;
        _currentUserProvider = currentUserProvider;
    }

    public ChatClientMetadata Metadata => _defaultChatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata ?? new ChatClientMetadata("Default", null);

    public async Task<IChatClient> GetClientAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default)
    {
        var model = await SelectModelAsync(criteria, cancellationToken);
        var userId = _currentUserProvider.GetCurrentUserId();
        
        UserApiKey? userKey = null;
        if (userId != Guid.Empty)
        {
            var provider = _modelService.GetProviderName(model.Id);
            userKey = await _apiKeyService.GetActiveKeyForProviderAsync(userId.Value, provider, cancellationToken);
        }

        // In a real system, if userKey is not null, we would instantiate a client with that key.
        // For now, we wrap the default client and include the key info in metadata if present.
        var client = userKey != null 
            ? new ChatClientMetadataWrapper(_defaultChatClient, model.Id.Value, userKey.ProviderName, userKey.Id.Value)
            : new ChatClientMetadataWrapper(_defaultChatClient, model.Id.Value);

        // Wrap with Usage Logging
        return new UsageLoggingChatClient(client, _usageService, userId.Value, model.Id, userKey?.Id.Value);
    }

    public async Task<AiModel> SelectModelAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default)
    {
        criteria ??= ModelCriteria.Default;
        var models = await _modelService.GetActiveModelsAsync(cancellationToken);

        var selected = models
            .Where(m => string.IsNullOrEmpty(criteria.ModelId) || m.Id.Value == criteria.ModelId)
            .Where(m => !criteria.MaxCostPerToken.HasValue || _modelService.GetCostPerToken(m.Id) <= criteria.MaxCostPerToken.Value)
            .OrderBy(m => _modelService.GetCostPerToken(m.Id))
            .FirstOrDefault();

        if (selected == null)
        {
            selected = models.FirstOrDefault() 
                ?? throw new InvalidOperationException("No active AI models available.");
        }

        return selected;
    }
}

internal class ChatClientMetadataWrapper : IChatClient
{
    private readonly IChatClient _innerClient;
    private readonly ChatClientMetadata _metadata;

    public ChatClientMetadataWrapper(IChatClient innerClient, string modelId, string? providerName = null, Guid? apiKeyId = null)
    {
        _innerClient = innerClient;
        var innerMetadata = innerClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        _metadata = new ChatClientMetadata(
            providerName ?? innerMetadata?.ProviderName ?? "UnknownProvider",
            null, // Metadata doesn't have BaseUri
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

internal class UsageLoggingChatClient : IChatClient
{
    private readonly IChatClient _innerClient;
    private readonly IUsageService _usageService;
    private readonly Guid _userId;
    private readonly ModelId _modelId;
    private readonly Guid? _apiKeyId;

    public UsageLoggingChatClient(IChatClient innerClient, IUsageService usageService, Guid userId, ModelId modelId, Guid? apiKeyId)
    {
        _innerClient = innerClient;
        _usageService = usageService;
        _userId = userId;
        _modelId = modelId;
        _apiKeyId = apiKeyId;
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await _innerClient.GetResponseAsync(chatMessages, options, cancellationToken);
        
        // Log usage
        if (response.Usage != null)
        {
            var metadata = _innerClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            await _usageService.LogUsageAsync(_userId, _modelId, (int)(response.Usage.TotalTokenCount ?? 0), 0, metadata?.ProviderName, _apiKeyId, cancellationToken);
        }

        return response;
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int totalTokens = 0;
        await foreach (var update in _innerClient.GetStreamingResponseAsync(chatMessages, options, cancellationToken))
        {
            if (update.Contents.Count > 0) // Simplified token counting for streaming
            {
                // In a real app, tracking tokens in streaming is more complex.
                // Microsoft.Extensions.AI often provides usage in the final update.
            }
            yield return update;
        }
        
        // Final logging if usage info is available at the end of stream (provider dependent)
    }

    public object? GetService(Type serviceType, object? key = null) => _innerClient.GetService(serviceType, key);
    public void Dispose() => _innerClient.Dispose();
}




