using FluentValidation;

namespace ChatBot.Features.SendMessage.V1;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Sender).NotEmpty();
        RuleFor(x => x.TokenUsed).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}
