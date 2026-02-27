using AI.Common.Core;
using AiOrchestration.Services;
using Ardalis.GuardClauses;

namespace AiOrchestration.Features.Models.GetModels.V1.Request;

internal class GetModelsHandler : IQueryHandler<GetModelsQuery, GetModelsQueryResult>
{
    private readonly IAiModelService _modelService;

    public GetModelsHandler(IAiModelService modelService)
    {
        _modelService = modelService;
    }

    public async Task<GetModelsQueryResult> Handle(GetModelsQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        var models = await _modelService.GetActiveModelsAsync(cancellationToken);

        return new GetModelsQueryResult(models);
    }
}
