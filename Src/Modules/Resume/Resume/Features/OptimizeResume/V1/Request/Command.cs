using AI.Common.Core;

namespace Resume.Features.OptimizeResume.V1;

public record OptimizeResumeCommand(string ResumeContent, string JobDescription, string? ModelId = null) : ICommand<OptimizeResumeCommandResult>;

public record OptimizeResumeCommandResult(Guid ResultId, string OptimizedResume, string ModelId, string? ProviderName);
