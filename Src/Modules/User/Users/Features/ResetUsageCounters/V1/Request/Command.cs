using AI.Common.Core;
using MassTransit;

namespace User.Features.ResetUsageCounters.V1;

public record ResetUsageCounterCommand(Guid UserId) : ICommand<ResetUsageCounterCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record ResetUsageCounterCommandResponse(Guid Id);
