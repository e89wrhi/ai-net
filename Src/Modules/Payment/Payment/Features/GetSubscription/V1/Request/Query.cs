using AI.Common.Caching;
using AI.Common.Core;
using Payment.Dtos;

namespace Payment.Features.GetSubscription.V1;

public record GetSubscription(Guid UserId) : IQuery<GetSubscriptionResult>, ICacheRequest
{
    public string CacheKey => $"GetSubscription_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetSubscriptionResult(SubscriptionDto SubscriptionDto);

public record GetSubscriptionResponseDto(SubscriptionDto SubscriptionDto);

