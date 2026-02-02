using FluentValidation;

namespace ChatBot.Features.StartChat.V1;

public class StartChatCommandValidator : AbstractValidator<StartChatCommand>
{
    public StartChatCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}
