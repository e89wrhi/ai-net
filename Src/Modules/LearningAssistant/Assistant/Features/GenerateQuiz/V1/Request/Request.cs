namespace LearningAssistant.Features.GenerateQuiz.V1;

public record GenerateAIQuizRequestDto(string Topic, int QuestionCount);
public record GenerateAIQuizResponseDto(Guid SessionId, Guid ActivityId, string QuizContent);
