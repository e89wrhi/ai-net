using FluentValidation;

namespace LearningAssistant.Features.GenerateLesson.V1;

public class GenerateAILessonCommandValidator : AbstractValidator<GenerateLessonCommand>
{
    public GenerateAILessonCommandValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.DifficultyLevel).NotEmpty();
    }
}
