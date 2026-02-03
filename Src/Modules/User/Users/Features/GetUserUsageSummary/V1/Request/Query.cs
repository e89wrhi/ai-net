using AI.Common.Caching;
using AI.Common.Core;
using User.Dtos;

namespace User.Features.GetUserUsageSummary.V1;

public record GetUserUsageSummary(Guid UserId) : IQuery<GetUserUsageSummaryResult>, ICacheRequest
{
    public string CacheKey => $"GetUserUsageSummary_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetUserUsageSummaryResult(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

public record GetUserUsageSummaryResponseDto(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

