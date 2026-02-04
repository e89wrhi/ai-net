using AI.Common.Core;
using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateLesson.V1;

public record GenerateLessonCommand(string Topic, DifficultyLevel Level, string? ModelId = null) : ICommand<GenerateLessonCommandResult>;

public record GenerateLessonCommandResult(Guid SessionId, Guid ActivityId, string Content, string ModelId, string? ProviderName);
