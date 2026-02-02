using FluentValidation;

namespace User.Features.ResetUsageCounters.V1;

public class ResetUsageCounterCommandValidator : AbstractValidator<ResetUsageCounterCommand>
{
    public ResetUsageCounterCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}