using FluentValidation;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public class StreamAnalyzeCodeCommandValidator : AbstractValidator<StreamAnalyzeCodeCommand>
{
    public StreamAnalyzeCodeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Language).IsInEnum();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
