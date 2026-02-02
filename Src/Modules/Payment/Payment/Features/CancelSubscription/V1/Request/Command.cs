using AI.Common.Core;
using MassTransit;

namespace Payment.Features.CancelSubscription.V1;

public record CancelSubscriptionCommand(Guid SubscriptionId) : ICommand<CancelSubscriptionCommandResponse>
{
}

public record CancelSubscriptionCommandResponse(Guid Id);
