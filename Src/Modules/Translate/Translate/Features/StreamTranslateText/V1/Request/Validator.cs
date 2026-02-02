using FluentValidation;

namespace Translate.Features.StreamTranslateText.V1;

public class StreamTranslateTextCommandValidator : AbstractValidator<StreamTranslateTextCommand>
{
    public StreamTranslateTextCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.SourceLanguage).NotEmpty();
        RuleFor(x => x.TargetLanguage).NotEmpty();
    }
}
