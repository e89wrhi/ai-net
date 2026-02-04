using LearningAssistant.Enums;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateQuizRequestDto(string Topic, int QuestionCount, LearningMode Mode, DifficultyLevel DifficultyLevel, string? ModelId = null);
public record GenerateQuizResponseDto(Guid SessionId, Guid ActivityId, string QuizContent, string ModelId, string? ProviderName);
