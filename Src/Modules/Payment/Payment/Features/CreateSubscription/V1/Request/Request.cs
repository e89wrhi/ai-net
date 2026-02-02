using Payment.ValueObjects;

namespace Payment.Features.CreateSubscription.V1;

public record CreateSubscriptionRequest(Guid UserId, SubscriptionPlan Plan, int MaxRequestsPerDay, int MaxTokensPerMonth);
public record CreateSubscriptionRequestResponse(Guid Id);
