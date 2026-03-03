using AI.Common.Core;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using MediatR;

namespace AiOrchestration.Features.ApiKeys.RemoveApiKey.V1.Request;

internal class RemoveApiKeyHandler : ICommandHandler<RemoveApiKeyCommand, RemoveApiKeyCommandResponse>
{
    private readonly IApiKeyService _apiKeyService;

    public RemoveApiKeyHandler(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    public async Task<RemoveApiKeyCommandResponse> Handle(RemoveApiKeyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        await _apiKeyService.DeleteKeyAsync(ApiKeyId.Of(request.Id), cancellationToken);
        return new RemoveApiKeyCommandResponse(request.Id);
    }
}
