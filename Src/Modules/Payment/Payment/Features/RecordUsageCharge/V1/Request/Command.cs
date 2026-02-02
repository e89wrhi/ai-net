using AI.Common.Core;
using MassTransit;

namespace Payment.Features.RecordUsageCharge.V1;

public record RecordUsageChargeCommand(Guid SubscriptionId, string TokenUsed, string Description, decimal Cost, string Currency, string Module) : ICommand<RecordUsageChargeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record RecordUsageChargeCommandResponse(Guid Id);
