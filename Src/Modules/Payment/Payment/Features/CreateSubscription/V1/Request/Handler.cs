using MassTransit;
using MediatR;
using Payment.Data;
using Payment.Models;
using Payment.ValueObjects;

namespace Payment.Features.CreateSubscription.V1;


internal class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public CreateSubscriptionHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateSubscriptionCommandResponse> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = SubscriptionModel.Create(
            SubscriptionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Plan.ToString(),
            request.MaxRequestsPerDay,
            request.MaxTokensPerMonth);

        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSubscriptionCommandResponse(subscription.Id.Value);
    }
}

