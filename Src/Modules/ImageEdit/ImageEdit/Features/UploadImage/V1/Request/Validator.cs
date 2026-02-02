using FluentValidation;

namespace ImageEdit.Features.UploadImage.V1;

public class StartImageEditCommandValidator : AbstractValidator<StartImageEditCommand>
{
    public StartImageEditCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}

