using FluentValidation;

namespace SimplePlugin.Features.StreamGenerateResponse.V1;

public class StreamGenerateResponseWithAICommandValidator : AbstractValidator<StreamGenerateResponseCommand>
{
    public StreamGenerateResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(4000).WithMessage("Text must not exceed 4 000 characters.");
    }
}
