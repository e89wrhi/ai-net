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

public record CancelSubscriptionMongo() : InternalCommand;

public class CancelSubscriptionMongoHandler : ICommandHandler<CancelSubscriptionMongo>
{
    private readonly PaymentReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public CancelSubscriptionMongoHandler(
        PaymentReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CancelSubscriptionMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<SubscriptionReadModel>(request);


        return Unit.Value;
    }
}
