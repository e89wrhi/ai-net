using FluentValidation;

namespace AutoComplete.Features.StreamAICompletion.V1;

public class StreamAutoCompleteCommandValidator : AbstractValidator<StreamAutoCompleteCommand>
{
    public StreamAutoCompleteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Prompt).NotEmpty();
    }
}
