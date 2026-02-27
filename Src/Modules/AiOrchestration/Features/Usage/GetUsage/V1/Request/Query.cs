using AI.Common.Core;
using AiOrchestration.Models;

namespace AiOrchestration.Features.Usage.GetUsage.V1.Request;

public record GetUsageQuery(DateTime? From = null, DateTime? To = null) : IQuery<GetUsageQueryResult>;

public record GetUsageQueryResult(IEnumerable<AiUsage> Usage);
