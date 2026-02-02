using FluentValidation;

namespace Payment.Features.RecordUsageCharge.V1;


public class RecordUsageChargeCommandValidator : AbstractValidator<RecordUsageChargeCommand>
{
    public RecordUsageChargeCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
