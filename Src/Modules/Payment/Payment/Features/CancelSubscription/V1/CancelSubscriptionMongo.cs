namespace Payment.Features.CancelSubscription.V1;


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

public record CancelSubscriptionMongo(Guid Id, string Status) : InternalCommand;

public class CancelSubscriptionMongoHandler : ICommandHandler<CancelSubscriptionMongo>
{
    private readonly PaymentReadDbContext _readDbContext;

    public CancelSubscriptionMongoHandler(PaymentReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(CancelSubscriptionMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<SubscriptionReadModel>.Filter.Eq(x => x.Id, request.Id);
        var update = Builders<SubscriptionReadModel>.Update.Set(x => x.Status, request.Status);

        await _readDbContext.Subscription.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

