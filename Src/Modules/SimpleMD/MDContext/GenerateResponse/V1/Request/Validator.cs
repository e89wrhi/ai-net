using FluentValidation;

namespace SimpleMD.Features.GenerateResponse.V1;

public class GenerateResponseWithAICommandValidator : AbstractValidator<GenerateResponseCommand>
{
    public GenerateResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(4000).WithMessage("Message must not exceed 4 000 characters.");
    }
}
