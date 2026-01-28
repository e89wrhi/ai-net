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

public record ResetUsageCountersMongo() : InternalCommand;

public class ResetUsageCountersMongoHandler : ICommandHandler<ResetUsageCountersMongo>
{
    private readonly UserReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public ResetUsageCountersMongoHandler(
        UserReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(ResetUsageCountersMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<UserReadModel>(request);


        return Unit.Value;
    }
}
