namespace LearningAssistant.Features.GenerateLesson.V1;


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

public record GenerateLessonMongo() : InternalCommand;

public class GenerateLessonMongoHandler : ICommandHandler<GenerateLessonMongo>
{
    private readonly ProfileReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public GenerateLessonMongoHandler(
        ProfileReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(GenerateLessonMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ProfileReadModel>(request);


        return Unit.Value;
    }
}
