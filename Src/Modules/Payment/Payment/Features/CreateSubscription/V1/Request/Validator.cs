using FluentValidation;

namespace Payment.Features.CreateSubscription.V1;


public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
