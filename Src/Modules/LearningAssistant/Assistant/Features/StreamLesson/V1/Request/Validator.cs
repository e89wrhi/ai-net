using FluentValidation;

namespace LearningAssistant.Features.StreamLesson.V1;

public class StreamAILessonCommandValidator : AbstractValidator<StreamAILessonCommand>
{
    public StreamAILessonCommandValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.Level).NotEmpty();
    }
}
