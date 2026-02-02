using FluentValidation;

namespace User.Features.AnalyzeUserUsage.V1;

public class AnalyzeUserUsageWithAICommandValidator : AbstractValidator<AnalyzeUserUsageWithAICommand>
{
    public AnalyzeUserUsageWithAICommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
