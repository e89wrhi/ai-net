using AI.Common.Core;
using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateLesson.V1;

public record GenerateAILessonCommand(string Topic, DifficultyLevel Level) : ICommand<GenerateAILessonCommandResult>;

public record GenerateAILessonCommandResult(Guid SessionId, Guid ActivityId, string Content);
