using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateLesson.V1;

public record GenerateLessonRequestDto(string Topic, DifficultyLevel Level, string? ModelId = null);
public record GenerateLessonResponseDto(Guid SessionId, Guid ActivityId, string Content, string ModelId, string? ProviderName);
