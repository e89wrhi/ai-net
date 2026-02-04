using FluentValidation;

namespace ImageCaption.Features.CaptionImage.V1;


public class AIImageCaptionCommandValidator : AbstractValidator<ImageCaptionCommand>
{
    public AIImageCaptionCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
    }
}
