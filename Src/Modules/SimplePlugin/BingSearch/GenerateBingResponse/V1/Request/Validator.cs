using FluentValidation;

namespace SimplePlugin.Features.GenerateBingResponse.V1;

public class GenerateBingResponseWithAICommandValidator : AbstractValidator<GenerateBingResponseCommand>
{
    public GenerateBingResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
