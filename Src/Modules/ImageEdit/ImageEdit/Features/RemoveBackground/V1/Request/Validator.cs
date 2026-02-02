using FluentValidation;

namespace ImageEdit.Features.RemoveBackground.V1;


public class RemoveBackgroundCommandValidator : AbstractValidator<RemoveBackgroundCommand>
{
    public RemoveBackgroundCommandValidator()
    {
        RuleFor(x => x.ImageUrlOrBase64).NotEmpty();
    }
}
