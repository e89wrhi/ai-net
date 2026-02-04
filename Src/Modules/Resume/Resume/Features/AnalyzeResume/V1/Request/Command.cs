using AI.Common.Core;

namespace Resume.Features.AnalyzeResume.V1;

public record AnalyzeResumeCommand(Guid UserId, string ResumeContent, string? ModelId = null) : ICommand<AnalyzeResumeCommandResult>;

public record AnalyzeResumeCommandResult(Guid SessionId, Guid ResultId, string Summary, double Score, string ModelId, string? ProviderName);
