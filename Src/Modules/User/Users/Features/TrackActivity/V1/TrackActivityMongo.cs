namespace User.Features.TrackActivity.V1;


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

public record TrackActivityMongo(Guid Id, Guid UserId, string Module, string Action, Guid ResourceId, DateTimeOffset TimeStamp) : InternalCommand;

public class TrackActivityMongoHandler : ICommandHandler<TrackActivityMongo>
{
    private readonly UserReadDbContext _readDbContext;

    public TrackActivityMongoHandler(UserReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(TrackActivityMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<UserAnalyticsReadModel>.Filter.Eq(x => x.Id, request.UserId);
        var activity = new UserActivityReadModel
        {
            Id = request.Id,
            Module = request.Module,
            Action = request.Action,
            ResourceId = request.ResourceId,
            TimeStamp = request.TimeStamp
        };

        var update = Builders<UserAnalyticsReadModel>.Update.Push(x => x.Activities, activity);

        await _readDbContext.User.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

