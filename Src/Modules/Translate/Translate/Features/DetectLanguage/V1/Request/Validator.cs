using FluentValidation;

namespace Translate.Features.DetectLanguage.V1;

public class DetectLanguageCommandValidator : AbstractValidator<DetectLanguageCommand>
{
    public DetectLanguageCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
