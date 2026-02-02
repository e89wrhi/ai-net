using FluentValidation;

namespace Payment.Features.CancelSubscription.V1;


public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}
