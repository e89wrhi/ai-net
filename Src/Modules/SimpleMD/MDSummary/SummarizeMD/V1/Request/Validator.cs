using FluentValidation;

namespace SimpleMD.Features.SummarizeMD.V1;

public class SummarizeMDWithAICommandValidator : AbstractValidator<SummarizeMDCommand>
{
    public SummarizeMDWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
