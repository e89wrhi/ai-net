using FluentValidation;

namespace ChatBot.Features.StreamResponse.V1;

public class StreamAiResponseCommandValidator : AbstractValidator<StreamAiResponseCommand>
{
    public StreamAiResponseCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}