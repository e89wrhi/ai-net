using AI.Common.Core;

namespace Resume.Features.OptimizeResume.V1;

public record OptimizeResumeWithAICommand(string ResumeContent, string JobDescription) : ICommand<OptimizeResumeWithAICommandResult>;

public record OptimizeResumeWithAICommandResult(Guid ResultId, string OptimizedResume);
