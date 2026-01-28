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
using MongoDB.Bson;


public record SubmitQuizeMongo(Guid QuizId, string UserAnswer, bool IsCorrect) : InternalCommand;

public class SubmitQuizeMongoHandler : ICommandHandler<SubmitQuizeMongo>
{
    private readonly ProfileReadDbContext _readDbContext;

    public SubmitQuizeMongoHandler(ProfileReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SubmitQuizeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Using arrayFilters to update a deeply nested element by its ID
        var filter = Builders<ProfileReadModel>.Filter.Empty;
        var update = Builders<ProfileReadModel>.Update
            .Set("Lessons.$[].Quizes.$[q].UserAnswer", request.UserAnswer)
            .Set("Lessons.$[].Quizes.$[q].IsCorrect", request.IsCorrect);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("q.Id", request.QuizId))
        };

        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

        await _readDbContext.Profile.UpdateManyAsync(filter, update, updateOptions, cancellationToken);

        return Unit.Value;
    }
}

