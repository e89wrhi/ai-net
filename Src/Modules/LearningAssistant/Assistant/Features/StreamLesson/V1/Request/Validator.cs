using FluentValidation;

namespace LearningAssistant.Features.StreamLesson.V1;

public class StreamAILessonCommandValidator : AbstractValidator<StreamAILessonCommand>
{
    public StreamAILessonCommandValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.DifficultyLevel).IsInEnum();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
