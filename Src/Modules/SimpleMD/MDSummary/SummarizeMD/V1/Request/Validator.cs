using FluentValidation;

namespace SimpleMD.Features.SummarizeMD.V1;

public class SummarizeMDWithAICommandValidator : AbstractValidator<SummarizeMDCommand>
{
    public SummarizeMDWithAICommandValidator()
    {
        RuleFor(x => x.Instruction)
            .NotEmpty()
            .MaximumLength(500).WithMessage("Instruction must not exceed 500 characters.");
    }
}
