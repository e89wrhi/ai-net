using FluentValidation;

namespace Payment.Features.ForecastSpendingWithAI.V1;


public class ForecastSpendingWithAICommandValidator : AbstractValidator<ForecastSpendingWithAICommand>
{
    public ForecastSpendingWithAICommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
