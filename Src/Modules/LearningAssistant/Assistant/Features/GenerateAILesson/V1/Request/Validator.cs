using FluentValidation;

namespace LearningAssistant.Features.GenerateAILesson.V1;

public class GenerateAILessonCommandValidator : AbstractValidator<GenerateAILessonCommand>
{
    public GenerateAILessonCommandValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.Level).NotEmpty();
    }
}
