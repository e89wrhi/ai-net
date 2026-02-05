using FluentValidation;

namespace ChatBot.Features.UpdateChat.V1;

public class UpdateChatValidator : AbstractValidator<UpdateChatCommand>
{
    public UpdateChatValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");
    }
}
