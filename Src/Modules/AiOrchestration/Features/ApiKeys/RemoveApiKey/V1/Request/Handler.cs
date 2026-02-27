using AI.Common.Core;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;

namespace AiOrchestration.Features.ApiKeys.RemoveApiKey.V1.Request;

internal class RemoveApiKeyHandler : ICommandHandler<RemoveApiKeyCommand>
{
    private readonly IApiKeyService _apiKeyService;

    public RemoveApiKeyHandler(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    public async Task Handle(RemoveApiKeyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        await _apiKeyService.DeleteKeyAsync(ApiKeyId.Of(request.Id), cancellationToken);
    }
}
