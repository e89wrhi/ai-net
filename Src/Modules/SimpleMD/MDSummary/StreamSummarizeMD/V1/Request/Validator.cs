using FluentValidation;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

public class StreamSummarizeMDWithAICommandValidator : AbstractValidator<StreamSummarizeMDCommand>
{
    public StreamSummarizeMDWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
