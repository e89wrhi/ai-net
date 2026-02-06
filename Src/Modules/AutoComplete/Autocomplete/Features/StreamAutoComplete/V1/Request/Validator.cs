using FluentValidation;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public class StreamAutoCompleteCommandValidator : AbstractValidator<StreamAutoCompleteCommand>
{
    public StreamAutoCompleteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.ModelId).MaximumLength(100).When(x => x.ModelId != null);

    }
}
