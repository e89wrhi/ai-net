using FluentValidation;

namespace Payment.Features.ForecastSpending.V1;


public class ForecastSpendingWithAICommandValidator : AbstractValidator<ForecastSpendingWithAICommand>
{
    public ForecastSpendingWithAICommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
