using FluentValidation;

namespace CodeGen.Features.ReGenerateCode.V1;


public class ReGenerateCodeCommandValidator : AbstractValidator<ReGenerateCodeCommand>
{
    public ReGenerateCodeCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Instruction).NotEmpty();
    }
}
