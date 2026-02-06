using FluentValidation;

namespace ChatBot.Features.StreamResponse.V1;

public class StreamAiResponseCommandValidator : AbstractValidator<StreamAiResponseCommand>
{
    public StreamAiResponseCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}