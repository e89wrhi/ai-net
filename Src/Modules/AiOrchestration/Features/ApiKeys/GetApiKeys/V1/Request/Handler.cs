using AI.Common.Core;
using AI.Common.Web;
using AiOrchestration.Dtos;
using AiOrchestration.Services;
using Ardalis.GuardClauses;

namespace AiOrchestration.Features.ApiKeys.GetApiKeys.V1.Request;

internal class GetApiKeysHandler : IQueryHandler<GetApiKeysQuery, GetApiKeysQueryResult>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetApiKeysHandler(IApiKeyService apiKeyService, ICurrentUserProvider currentUserProvider)
    {
        _apiKeyService = apiKeyService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<GetApiKeysQueryResult> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        var userId = _currentUserProvider.GetCurrentUserId();
        var keys = await _apiKeyService.GetKeysAsync(userId, cancellationToken);

        var dtos = keys.Select(k => new ApiKeyDto(
            k.Id.Value, 
            k.ProviderName, 
            k.Label, 
            k.IsActive, 
            k.LastUsedAt));

        return new GetApiKeysQueryResult(dtos);
    }
}
