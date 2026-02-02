namespace Payment.Features.CancelSubscription.V1;

public record CancelSubscriptionRequest(Guid SubscriptionId);
public record CancelSubscriptionRequestResponse(Guid Id);
