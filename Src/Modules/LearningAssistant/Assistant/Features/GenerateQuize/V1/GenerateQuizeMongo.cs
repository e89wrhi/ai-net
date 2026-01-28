namespace LearningAssistant.Features.GenerateQuize.V1;


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

public record GenerateQuizeMongo() : InternalCommand;

public class GenerateQuizeMongoHandler : ICommandHandler<GenerateQuizeMongo>
{
    private readonly ProfileReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public GenerateQuizeMongoHandler(
        ProfileReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(GenerateQuizeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ProfileReadModel>(request);


        return Unit.Value;
    }
}
