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

public record GenerateLessonMongo(Guid ProfileId, Guid LessonId, string Title, string Content) : InternalCommand;

public class GenerateLessonMongoHandler : ICommandHandler<GenerateLessonMongo>
{
    private readonly LearningReadDbContext _readDbContext;

    public GenerateLessonMongoHandler(LearningReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateLessonMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<LearningSessionReadModel>.Filter.Eq(x => x.Id, request.ProfileId);
        
        var lesson = new LessonReadModel
        {
            Id = request.LessonId,
            Title = request.Title,
            Content = request.Content,
            IsCompleted = false
        };

        var update = Builders<LearningSessionReadModel>.Update.Push(x => x.Lessons, lesson);

        await _readDbContext.Profiles.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

