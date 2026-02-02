using FluentValidation;

namespace ImageCaption.Features.GenerateCaption.V1;


public class GenerateCaptionCommandValidator : AbstractValidator<GenerateCaptionCommand>
{
    public GenerateCaptionCommandValidator()
    {
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.CaptionText).NotEmpty();
    }
}