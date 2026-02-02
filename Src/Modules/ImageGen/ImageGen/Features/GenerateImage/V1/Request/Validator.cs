using FluentValidation;

namespace ImageGen.Features.GenerateImage.V1;

public class GenerateImageCommandValidator : AbstractValidator<GenerateImageCommand>
{
    public GenerateImageCommandValidator()
    {
        RuleFor(x => x.Style).NotEmpty();
        RuleFor(x => x.Size).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
    }
}
