using AI.Common.Core;

namespace Resume.Features.OptimizeResume.V1;

public record OptimizeResumeCommand(Guid UserId, string ResumeContent, string JobDescription, bool IncludeSkill, bool IncludeEducation, bool IncludeExpireance, string? ModelId = null) : ICommand<OptimizeResumeCommandResult>;

public record OptimizeResumeCommandResult(Guid ResultId, string OptimizedResume, string ModelId, string? ProviderName);
