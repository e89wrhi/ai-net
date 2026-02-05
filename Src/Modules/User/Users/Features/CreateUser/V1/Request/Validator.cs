using FluentValidation;

namespace User.Features.CreateUser.V1;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
