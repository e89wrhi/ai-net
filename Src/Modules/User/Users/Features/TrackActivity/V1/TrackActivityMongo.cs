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

public record TrackActivityMongo() : InternalCommand;

public class TrackActivityMongoHandler : ICommandHandler<TrackActivityMongo>
{
    private readonly UserReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public TrackActivityMongoHandler(
        UserReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(TrackActivityMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<UserReadModel>(request);


        return Unit.Value;
    }
}
