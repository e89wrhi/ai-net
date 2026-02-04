using FluentValidation;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public class GenerateAIQuizCommandValidator : AbstractValidator<GenerateQuizCommand>
{
    public GenerateAIQuizCommandValidator()
    {
        RuleFor(x => x.QuestionCount).NotEmpty();
        RuleFor(x => x.Topic).NotEmpty();
    }
}
