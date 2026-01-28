namespace Payment.Features.CreateSubscription.V1;


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

public record CreateSubscriptionMongo(Guid Id, Guid UserId, string Plan, string Status, DateTime StartedAt, DateTime ExpiresAt) : InternalCommand;

public class CreateSubscriptionMongoHandler : ICommandHandler<CreateSubscriptionMongo>
{
    private readonly PaymentReadDbContext _readDbContext;

    public CreateSubscriptionMongoHandler(PaymentReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(CreateSubscriptionMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = new SubscriptionReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Plan = request.Plan,
            Status = request.Status,
            StartedAt = request.StartedAt,
            ExpiresAt = request.ExpiresAt
        };

        await _readDbContext.Subscription.InsertOneAsync(subscription, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

