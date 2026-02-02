using AI.Common.Core;

namespace Resume.Features.AnalyzeResumeWithAI.V1;

public record AnalyzeResumeWithAICommand(string ResumeContent) : ICommand<AnalyzeResumeWithAICommandResult>;

public record AnalyzeResumeWithAICommandResult(Guid SessionId, Guid ResultId, string Summary, double Score);
