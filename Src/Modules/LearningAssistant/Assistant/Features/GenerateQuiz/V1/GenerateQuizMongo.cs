using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using MediatR;
using MongoDB.Driver;
using AI.Common.Core;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateQuizMongo(Guid LessonId, Guid QuizId, string Question, string CorrectAnswer) : InternalCommand;

public class GenerateQuizMongoHandler : ICommandHandler<GenerateQuizMongo>
{
    private readonly ProfileReadDbContext _readDbContext;

    public GenerateQuizMongoHandler(ProfileReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateQuizMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ProfileReadModel>.Filter.ElemMatch(x => x.Lessons, l => l.Id == request.LessonId);
        
        var quiz = new QuizReadModel
        {
            Id = request.QuizId,
            Question = request.Question,
            CorrectAnswer = request.CorrectAnswer
        };

        var update = Builders<ProfileReadModel>.Update.Push("Lessons.$.Quizzes", quiz);

        await _readDbContext.Profiles.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
