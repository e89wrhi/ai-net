using AI.Common.Core;
using AiOrchestration.Models;

namespace AiOrchestration.Features.Models.GetModels.V1.Request;

public record GetModelsQuery() : IQuery<GetModelsQueryResult>;

public record GetModelsQueryResult(IEnumerable<AiModel> Models);
