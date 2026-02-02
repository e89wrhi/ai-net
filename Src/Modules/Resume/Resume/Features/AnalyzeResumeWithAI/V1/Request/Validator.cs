using FluentValidation;

namespace Resume.Features.AnalyzeResumeWithAI.V1;

public class AnalyzeResumeWithAICommandValidator : AbstractValidator<AnalyzeResumeWithAICommand>
{
    public AnalyzeResumeWithAICommandValidator()
    {
        RuleFor(x => x.ResumeContent).NotEmpty();
    }
}
