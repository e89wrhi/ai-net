namespace User.Features.AnalyzeUserUsage.V1;

public record AnalyzeUserUsageWithAIRequestDto(Guid UserId);
public record AnalyzeUserUsageWithAIResponseDto(string UsageSummary, string Recommendations);
