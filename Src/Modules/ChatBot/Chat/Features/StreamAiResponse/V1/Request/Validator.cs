using FluentValidation;

namespace ChatBot.Features.StreamAiResponse.V1;

public class StreamAiResponseCommandValidator : AbstractValidator<StreamAiResponseCommand>
{
    public StreamAiResponseCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}