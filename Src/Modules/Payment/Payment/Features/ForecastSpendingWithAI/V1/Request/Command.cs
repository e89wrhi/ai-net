using AI.Common.Core;

namespace Payment.Features.ForecastSpendingWithAI.V1;


public record ForecastSpendingWithAICommand(Guid UserId) : ICommand<ForecastSpendingWithAICommandResult>;

public record ForecastSpendingWithAICommandResult(decimal ForecastedAmount, string Currency, string Insights);
