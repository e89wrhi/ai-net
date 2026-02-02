using FluentValidation;

namespace LearningAssistant.Features.SubmitQuiz.V1;

public class SubmitQuizCommandValidator : AbstractValidator<SubmitQuizCommand>
{
    public SubmitQuizCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.QuizId).NotEmpty();
    }
}

