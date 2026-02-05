using Ardalis.GuardClauses;
using MassTransit;
using MediatR;
using Payment.Data;
using Payment.Exceptions;
using Payment.Models;
using Payment.ValueObjects;

namespace Payment.Features.RecordUsageCharge.V1;


internal class RecordUsageChargeHandler : IRequestHandler<RecordUsageChargeCommand, RecordUsageChargeCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public RecordUsageChargeHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RecordUsageChargeCommandResponse> Handle(RecordUsageChargeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions.FindAsync(new object[] { SubscriptionId.Of(request.SubscriptionId) }, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var charge = UsageCharge.Create(
            PaymentId.Of(NewId.NextGuid()),
            subscription.Id,
            subscription.UserId,
            request.TokenUsed,
            request.Description,
            Money.Of(request.Cost, request.Currency),
            request.Module);

        subscription.AddCharge(charge);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RecordUsageChargeCommandResponse(charge.Id.Value);
    }
}

