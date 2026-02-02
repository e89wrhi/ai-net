namespace User.Features.ResetUsageCounters.V1;


public record ResetUsageCounterRequest(Guid UserId);

public record ResetUsageCounterRequestResponse(Guid Id);
