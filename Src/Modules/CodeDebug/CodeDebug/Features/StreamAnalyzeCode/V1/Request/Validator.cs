using FluentValidation;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public class StreamAnalyzeCodeCommandValidator : AbstractValidator<StreamAnalyzeCodeCommand>
{
    public StreamAnalyzeCodeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
