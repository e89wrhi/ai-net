using FluentValidation;

namespace User.Features.GenerateUserPersona.V1;

public class GenerateUserPersonaWithAICommandValidator : AbstractValidator<GenerateUserPersonaWithAICommand>
{
    public GenerateUserPersonaWithAICommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
