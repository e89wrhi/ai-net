using FluentValidation;

namespace ImageCaption.Features.AIImageCaption.V1;


public class AIImageCaptionCommandValidator : AbstractValidator<AIImageCaptionCommand>
{
    public AIImageCaptionCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
    }
}
