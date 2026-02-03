using AI.Common.Caching;
using AI.Common.Core;
using User.Dtos;

namespace User.Features.GetUserActivity.V1;

public record GetUserActivity(Guid UserId) : IQuery<GetUserActivityResult>, ICacheRequest
{
    public string CacheKey => $"GetUserActivity_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetUserActivityResult(IEnumerable<UserActivityDto> UserActivityDtos);

public record GetUserActivityResponseDto(IEnumerable<UserActivityDto> UserActivityDtos);

