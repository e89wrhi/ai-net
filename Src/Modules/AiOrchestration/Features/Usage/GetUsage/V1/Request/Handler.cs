using AI.Common.Core;
using AI.Common.Web;
using AiOrchestration.Services;
using Ardalis.GuardClauses;

namespace AiOrchestration.Features.Usage.GetUsage.V1.Request;

internal class GetUsageHandler : IQueryHandler<GetUsageQuery, GetUsageQueryResult>
{
    private readonly IUsageService _usageService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetUsageHandler(IUsageService usageService, ICurrentUserProvider currentUserProvider)
    {
        _usageService = usageService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<GetUsageQueryResult> Handle(GetUsageQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        
        var userId = _currentUserProvider.GetCurrentUserId();
        if (userId is not null)
        {
            var usage = await _usageService.GetUsageAsync(userId.Value, request.From, request.To, cancellationToken);

            return new GetUsageQueryResult(usage);
        }
        else
        {
            return new GetUsageQueryResult(null);
        }
    }
}
