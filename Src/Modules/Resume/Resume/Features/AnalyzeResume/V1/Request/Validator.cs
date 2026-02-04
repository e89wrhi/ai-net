using FluentValidation;

namespace Resume.Features.AnalyzeResume.V1;

public class AnalyzeResumeWithAICommandValidator : AbstractValidator<AnalyzeResumeCommand>
{
    public AnalyzeResumeWithAICommandValidator()
    {
        RuleFor(x => x.ResumeContent).NotEmpty();
    }
}
