namespace LearningAssistant.Features.SubmitQuize.V1;


using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using LearningAssistant.Exceptions;
using System;

public record SubmitQuizeMongoMongo() : InternalCommand;

public class SubmitQuizeMongoMongoHandler : ICommandHandler<SubmitQuizeMongoMongo>
{
    private readonly ProfileReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public SubmitQuizeMongoMongoHandler(
        ProfileReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(SubmitQuizeMongoMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ProfileReadModel>(request);


        return Unit.Value;
    }
}
