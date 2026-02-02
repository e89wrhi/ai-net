using FluentValidation;

namespace ImageEdit.Features.EnhanceImage.V1;


public class AIEnhanceImageCommandValidator : AbstractValidator<AIEnhanceImageCommand>
{
    public AIEnhanceImageCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
    }
}
