using FluentValidation;

namespace SimplePlugin.Features.GenerateResponse.V1;

public class GenerateResponseWithAICommandValidator : AbstractValidator<GenerateResponseCommand>
{
    public GenerateResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
