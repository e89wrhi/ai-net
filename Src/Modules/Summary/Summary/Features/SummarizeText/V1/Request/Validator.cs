using FluentValidation;

namespace Summary.Features.SummarizeText.V1;

public class SummarizeTextWithAICommandValidator : AbstractValidator<SummarizeTextCommand>
{
    public SummarizeTextWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.DetailLevel).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
