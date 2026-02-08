using FluentValidation;

namespace SimplePlugin.Features.StreamGenerateResponse.V1;

public class StreamGenerateResponseWithAICommandValidator : AbstractValidator<StreamGenerateResponseCommand>
{
    public StreamGenerateResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
