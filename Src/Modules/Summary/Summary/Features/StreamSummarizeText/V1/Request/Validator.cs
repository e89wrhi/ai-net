using FluentValidation;

namespace Summary.Features.StreamSummarizeText.V1;

public class StreamSummarizeTextCommandValidator : AbstractValidator<StreamSummarizeTextCommand>
{
    public StreamSummarizeTextCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.DetailLevel).IsInEnum();
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
