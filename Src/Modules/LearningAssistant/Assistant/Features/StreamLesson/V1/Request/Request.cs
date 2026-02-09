using LearningAssistant.Enums;

namespace LearningAssistant.Features.StreamLesson.V1;

public record StreamAILessonRequestDto(Guid UserId, string Topic, LearningMode Mode, DifficultyLevel DifficultyLevel, string? ModelId = null);

