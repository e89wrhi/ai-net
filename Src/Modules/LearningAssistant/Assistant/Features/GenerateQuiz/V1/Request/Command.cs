using AI.Common.Core;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateAIQuizCommand(string Topic, int QuestionCount) : ICommand<GenerateAIQuizCommandResult>;

public record GenerateAIQuizCommandResult(Guid SessionId, Guid ActivityId, string QuizContent);

