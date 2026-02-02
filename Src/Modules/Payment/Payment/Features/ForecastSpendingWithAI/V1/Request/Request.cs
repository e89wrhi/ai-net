namespace Payment.Features.ForecastSpendingWithAI.V1;

public record ForecastSpendingWithAIRequestDto(Guid UserId);
public record ForecastSpendingWithAIResponseDto(decimal ForecastedAmount, string Currency, string Insights);
