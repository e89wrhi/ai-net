using FluentValidation;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public class StreamGenerateBingResponseWithAICommandValidator : AbstractValidator<StreamGenerateBingResponseCommand>
{
    public StreamGenerateBingResponseWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
