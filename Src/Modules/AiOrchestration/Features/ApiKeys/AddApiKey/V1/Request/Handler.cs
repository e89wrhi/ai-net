using AI.Common.Core;
using AI.Common.Web;
using AiOrchestration.Services;
using Ardalis.GuardClauses;

namespace AiOrchestration.Features.ApiKeys.AddApiKey.V1.Request;

internal class AddApiKeyHandler : ICommandHandler<AddApiKeyCommand, AddApiKeyCommandResult>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AddApiKeyHandler(IApiKeyService apiKeyService, ICurrentUserProvider currentUserProvider)
    {
        _apiKeyService = apiKeyService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<AddApiKeyCommandResult> Handle(AddApiKeyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        var userId = _currentUserProvider.GetCurrentUserId();
        var apiKey = await _apiKeyService.AddKeyAsync(userId, request.Provider, request.Key, request.Label, cancellationToken);

        return new AddApiKeyCommandResult(apiKey.Id.Value, apiKey.ProviderName, apiKey.Label);
    }
}
