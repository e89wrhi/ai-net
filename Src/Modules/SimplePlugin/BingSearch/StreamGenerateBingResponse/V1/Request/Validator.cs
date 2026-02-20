using FluentValidation;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public class StreamGenerateBingResponseWithAICommandValidator : AbstractValidator<StreamGenerateBingResponseCommand>
{
    public StreamGenerateBingResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(4000).WithMessage("Text must not exceed 4 000 characters.");
    }
}
