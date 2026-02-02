using FluentValidation;

namespace ImageGen.Features.ReGenerateImage.V1;

public class ReGenerateImageCommandValidator : AbstractValidator<ReGenerateImageCommand>
{
    public ReGenerateImageCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Instruction).NotEmpty();
    }
}

