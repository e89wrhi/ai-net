using AI.Common.Core;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateQuizCommand(string Topic, int QuestionCount, string? ModelId = null) : ICommand<GenerateQuizCommandResult>;

public record GenerateQuizCommandResult(Guid SessionId, Guid ActivityId, string QuizContent, string ModelId, string? ProviderName);

