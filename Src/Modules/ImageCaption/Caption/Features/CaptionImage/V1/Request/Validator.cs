using FluentValidation;

namespace ImageCaption.Features.CaptionImage.V1;


public class AIImageCaptionCommandValidator : AbstractValidator<AIImageCaptionCommand>
{
    public AIImageCaptionCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
    }
}
