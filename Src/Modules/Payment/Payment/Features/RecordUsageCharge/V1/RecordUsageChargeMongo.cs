namespace Payment.Features.RecordUsageCharge.V1;


using Ardalis.GuardClauses;
using Payment.Data;
using Payment.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Payment.Exceptions;
using System;

public record RecordUsageChargeMongo(Guid SubscriptionId, Guid ChargeId, decimal Amount, string Currency, string Module, string Description, DateTime CreatedAt) : InternalCommand;

public class RecordUsageChargeMongoHandler : ICommandHandler<RecordUsageChargeMongo>
{
    private readonly PaymentReadDbContext _readDbContext;

    public RecordUsageChargeMongoHandler(PaymentReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(RecordUsageChargeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<SubscriptionReadModel>.Filter.Eq(x => x.Id, request.SubscriptionId);
        
        var charge = new UsageChargeReadModel
        {
            Id = request.ChargeId,
            Amount = request.Amount,
            Currency = request.Currency,
            Module = request.Module,
            Description = request.Description,
            CreatedAt = request.CreatedAt
        };

        var update = Builders<SubscriptionReadModel>.Update.Push(x => x.Charges, charge);

        await _readDbContext.Subscription.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

