using FluentValidation;

namespace ImageCaption.Features.AnalyzeImage.V1;

public class AnalyzeImageCommandValidator : AbstractValidator<AnalyzeImageCommand>
{
    public AnalyzeImageCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
    }
}

