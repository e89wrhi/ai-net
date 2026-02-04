using AI.Common.Core;
using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateLesson.V1;

public record GenerateLessonCommand(Guid UserId, string Topic, LearningMode Mode, DifficultyLevel DifficultyLevel, string? ModelId = null) : ICommand<GenerateLessonCommandResult>;

public record GenerateLessonCommandResult(Guid SessionId, Guid ActivityId, string Content, string ModelId, string? ProviderName);
