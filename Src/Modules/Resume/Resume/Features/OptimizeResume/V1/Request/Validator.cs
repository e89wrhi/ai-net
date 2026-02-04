using FluentValidation;

namespace Resume.Features.OptimizeResume.V1;

public class OptimizeResumeWithAICommandValidator : AbstractValidator<OptimizeResumeCommand>
{
    public OptimizeResumeWithAICommandValidator()
    {
        RuleFor(x => x.ResumeContent).NotEmpty();
        RuleFor(x => x.JobDescription).NotEmpty();
    }
}
