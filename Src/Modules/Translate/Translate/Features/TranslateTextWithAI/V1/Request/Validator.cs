using FluentValidation;

namespace Translate.Features.TranslateTextWithAI.V1;

public class TranslateTextWithAICommandValidator : AbstractValidator<TranslateTextWithAICommand>
{
    public TranslateTextWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.TargetLanguage).NotEmpty();
        RuleFor(x => x.SourceLanguage).NotEmpty();
    }
}
