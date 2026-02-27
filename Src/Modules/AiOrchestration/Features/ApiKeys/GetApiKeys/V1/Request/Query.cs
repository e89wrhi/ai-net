using AI.Common.Core;
using AiOrchestration.Dtos;

namespace AiOrchestration.Features.ApiKeys.GetApiKeys.V1.Request;

public record GetApiKeysQuery() : IQuery<GetApiKeysQueryResult>;

public record GetApiKeysQueryResult(IEnumerable<ApiKeyDto> Keys);
