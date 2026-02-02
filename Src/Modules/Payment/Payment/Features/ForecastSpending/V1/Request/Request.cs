namespace Payment.Features.ForecastSpending.V1;

public record ForecastSpendingWithAIRequestDto(Guid UserId);
public record ForecastSpendingWithAIResponseDto(decimal ForecastedAmount, string Currency, string Insights);
