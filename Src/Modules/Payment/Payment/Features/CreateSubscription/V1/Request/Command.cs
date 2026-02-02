using AI.Common.Core;
using MassTransit;

namespace Payment.Features.CreateSubscription.V1;


public record CreateSubscriptionCommand(Guid UserId, Models.SubscriptionPlan Plan, int MaxRequestsPerDay, int MaxTokensPerMonth) : ICommand<CreateSubscriptionCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateSubscriptionCommandResponse(Guid Id);
