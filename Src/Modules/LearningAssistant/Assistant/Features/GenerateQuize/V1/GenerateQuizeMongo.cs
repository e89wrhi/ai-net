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

public record GenerateQuizeMongo(Guid LessonId, Guid QuizId, string Question, string CorrectAnswer) : InternalCommand;

public class GenerateQuizeMongoHandler : ICommandHandler<GenerateQuizeMongo>
{
    private readonly ProfileReadDbContext _readDbContext;

    public GenerateQuizeMongoHandler(ProfileReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateQuizeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ProfileReadModel>.Filter.ElemMatch(x => x.Lessons, l => l.Id == request.LessonId);
        
        var quiz = new QuizeReadModel
        {
            Id = request.QuizId,
            Question = request.Question,
            CorrectAnswer = request.CorrectAnswer
        };

        var update = Builders<ProfileReadModel>.Update.Push("Lessons.$.Quizes", quiz);

        await _readDbContext.Profile.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

