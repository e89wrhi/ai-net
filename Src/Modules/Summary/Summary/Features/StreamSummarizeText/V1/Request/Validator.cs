using FluentValidation;

namespace Summary.Features.StreamSummarizeText.V1;

public class StreamSummarizeTextCommandValidator : AbstractValidator<StreamSummarizeTextCommand>
{
    public StreamSummarizeTextCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.DetailLevel).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
