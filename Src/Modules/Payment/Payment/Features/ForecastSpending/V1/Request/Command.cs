using AI.Common.Core;

namespace Payment.Features.ForecastSpending.V1;

public record ForecastSpendingWithAICommand(Guid UserId) : ICommand<ForecastSpendingWithAICommandResult>;

public record ForecastSpendingWithAICommandResult(decimal ForecastedAmount, string Currency, string Insights);
