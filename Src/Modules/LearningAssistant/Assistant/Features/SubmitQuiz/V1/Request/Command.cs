using AI.Common.Core;
using MassTransit;

namespace LearningAssistant.Features.SubmitQuiz.V1;

public record SubmitQuizCommand(Guid UserId, Guid LessonId, Guid QuizId, double Score) : ICommand<SubmitQuizCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SubmitQuizCommandResponse(Guid Id);
