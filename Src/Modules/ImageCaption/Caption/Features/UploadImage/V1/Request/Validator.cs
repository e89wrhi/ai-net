using FluentValidation;

namespace ImageCaption.Features.UploadImage.V1;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ImageUrl).NotEmpty();
    }
}

