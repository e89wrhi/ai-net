namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateQuizRequestDto(string Topic, int QuestionCount, string? ModelId = null);
public record GenerateQuizResponseDto(Guid SessionId, Guid ActivityId, string QuizContent, string ModelId, string? ProviderName);
