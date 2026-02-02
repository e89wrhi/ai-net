using FluentValidation;

namespace ChatBot.Features.DeleteChat.V1;


public class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
{
    public DeleteChatCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
