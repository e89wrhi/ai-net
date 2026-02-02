using FluentValidation;

namespace User.Features.UpdateUser.V1;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
