namespace LearningAssistant.Features.SubmitQuiz.V1;


public record SubmitQuizRequest(Guid LessonId, Guid QuizId, double Score);

public record SubmitQuizRequestResponse(Guid Id);
