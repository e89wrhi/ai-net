namespace User.Features.ResetUsageCounters.V1;


using Ardalis.GuardClauses;
using User.Data;
using User.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using User.Exceptions;
using System;

public record ResetUsageCountersMongo(Guid UserId) : InternalCommand;

public class ResetUsageCountersMongoHandler : ICommandHandler<ResetUsageCountersMongo>
{
    private readonly UserReadDbContext _readDbContext;

    public ResetUsageCountersMongoHandler(UserReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ResetUsageCountersMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<UserAnalyticsReadModel>.Filter.Eq(x => x.Id, request.UserId);
        var update = Builders<UserAnalyticsReadModel>.Update.Set(x => x.Usages, new List<UsageContainerReadModel>());

        await _readDbContext.User.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

