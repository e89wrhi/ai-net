using FluentValidation;

namespace ChatBot.Features.GenerateAiResponse.V1;

public class GenerateAiResponseCommandValidator : AbstractValidator<GenerateAiResponseCommand>
{
    public GenerateAiResponseCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}