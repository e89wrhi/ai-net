using FluentValidation;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

public class StreamSummarizeMDWithAICommandValidator : AbstractValidator<StreamSummarizeMDCommand>
{
    public StreamSummarizeMDWithAICommandValidator()
    {
        RuleFor(x => x.Instruction)
            .NotEmpty()
            .MaximumLength(500).WithMessage("Instruction must not exceed 500 characters.");
    }
}
