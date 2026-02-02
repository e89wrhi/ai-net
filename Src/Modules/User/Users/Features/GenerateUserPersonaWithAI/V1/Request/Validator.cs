using FluentValidation;

namespace User.Features.GenerateUserPersonaWithAI.V1;

public class GenerateUserPersonaWithAICommandValidator : AbstractValidator<GenerateUserPersonaWithAICommand>
{
    public GenerateUserPersonaWithAICommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
