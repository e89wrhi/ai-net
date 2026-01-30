using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using MediatR;
using MongoDB.Driver;
using AI.Common.Core;
using MongoDB.Bson;

namespace LearningAssistant.Features.SubmitQuiz.V1;


public record SubmitQuizMongo(Guid QuizId, string UserAnswer, bool IsCorrect) : InternalCommand;

public class SubmitQuizMongoHandler : ICommandHandler<SubmitQuizMongo>
{
    private readonly ProfileReadDbContext _readDbContext;

    public SubmitQuizMongoHandler(ProfileReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SubmitQuizMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Using arrayFilters to update a deeply nested element by its ID
        var filter = Builders<ProfileReadModel>.Filter.Empty;
        var update = Builders<ProfileReadModel>.Update
            .Set("Lessons.$[].Quizzes.$[q].UserAnswer", request.UserAnswer)
            .Set("Lessons.$[].Quizzes.$[q].IsCorrect", request.IsCorrect);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument())
        };

        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

        await _readDbContext.Profiles.UpdateManyAsync(filter, update, updateOptions, cancellationToken);

        return Unit.Value;
    }
}
