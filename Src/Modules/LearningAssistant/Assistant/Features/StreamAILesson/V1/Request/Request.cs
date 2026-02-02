using LearningAssistant.Enums;

namespace LearningAssistant.Features.StreamAILesson.V1;


public record StreamAILessonRequestDto(string Topic, DifficultyLevel Level);
