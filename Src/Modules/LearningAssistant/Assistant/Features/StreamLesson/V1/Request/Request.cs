using LearningAssistant.Enums;

namespace LearningAssistant.Features.StreamLesson.V1;

public record StreamAILessonRequestDto(string Topic, DifficultyLevel Level);
