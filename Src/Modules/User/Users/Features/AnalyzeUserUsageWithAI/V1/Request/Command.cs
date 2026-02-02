using AI.Common.Core;

namespace User.Features.AnalyzeUserUsage.V1;


public record AnalyzeUserUsageWithAICommand(Guid UserId) : ICommand<AnalyzeUserUsageWithAICommandResult>;

public record AnalyzeUserUsageWithAICommandResult(string UsageSummary, string Recommendations);
