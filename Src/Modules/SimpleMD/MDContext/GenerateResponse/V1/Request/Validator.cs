using FluentValidation;

namespace SimpleMD.Features.GenerateResponse.V1;

public class GenerateResponseWithAICommandValidator : AbstractValidator<GenerateResponseCommand>
{
    public GenerateResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
