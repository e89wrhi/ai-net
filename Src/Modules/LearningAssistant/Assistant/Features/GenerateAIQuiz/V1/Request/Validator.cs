using FluentValidation;

namespace LearningAssistant.Features.GenerateAIQuize.V1;

public class GenerateAIQuizCommandValidator : AbstractValidator<GenerateAIQuizCommand>
{
    public GenerateAIQuizCommandValidator()
    {
        RuleFor(x => x.QuestionCount).NotEmpty();
        RuleFor(x => x.Topic).NotEmpty();
    }
}
