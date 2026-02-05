using Ardalis.GuardClauses;
using MediatR;
using Payment.Data;
using Payment.Exceptions;
using Payment.ValueObjects;

namespace Payment.Features.CancelSubscription.V1;


internal class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public CancelSubscriptionHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CancelSubscriptionCommandResponse> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions.FindAsync(new object[] { SubscriptionId.Of(request.SubscriptionId) }, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        subscription.Cancel();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CancelSubscriptionCommandResponse(subscription.Id.Value);
    }
}

