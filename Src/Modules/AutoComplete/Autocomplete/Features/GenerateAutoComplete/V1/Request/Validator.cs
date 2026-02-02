using FluentValidation;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public class GenerateAutoCompleteCommandValidator : AbstractValidator<GenerateAutoCompleteCommand>
{
    public GenerateAutoCompleteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
    }
}