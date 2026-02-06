using FluentValidation;

namespace CodeGen.Features.StreamGenerateCode.V1;

public class StreamGenerateCodeCommandValidator : AbstractValidator<StreamGenerateCodeCommand>
{
    public StreamGenerateCodeCommandValidator()
    {
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
