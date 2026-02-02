using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateAILesson.V1;

public record GenerateAILessonRequestDto(string Topic, DifficultyLevel Level);
public record GenerateAILessonResponseDto(Guid SessionId, Guid ActivityId, string Content);
