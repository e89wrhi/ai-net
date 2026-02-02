using FluentValidation;

namespace CodeGen.Features.GenerateCode.V1;

public class GenerateCodeCommandValidator : AbstractValidator<GenerateCodeCommand>
{
    public GenerateCodeCommandValidator()
    {
        RuleFor(x => x.Prompt).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
