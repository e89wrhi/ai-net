using FluentValidation;

namespace CodeDebug.Features.AnalyzeCode.V1;

public class AnalyzeCodeCommandValidator : AbstractValidator<AnalyzeCodeCommand>
{
    public AnalyzeCodeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
