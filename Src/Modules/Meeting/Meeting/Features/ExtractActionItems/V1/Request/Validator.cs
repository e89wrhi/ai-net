using FluentValidation;

namespace Meeting.Features.ExtractActionItems.V1;

public class ExtractActionItemsCommandValidator : AbstractValidator<ExtractActionItemsCommand>
{
    public ExtractActionItemsCommandValidator()
    {
        RuleFor(x => x.Transcript).NotEmpty();
    }
}
