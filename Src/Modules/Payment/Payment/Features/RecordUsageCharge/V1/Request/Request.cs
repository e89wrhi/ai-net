namespace Payment.Features.RecordUsageCharge.V1;

public record RecordUsageChargeRequest(Guid SubscriptionId, string TokenUsed, string Description, decimal Cost, string Currency, string Module);
public record RecordUsageChargeRequestResponse(Guid Id);
