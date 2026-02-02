namespace Resume.Features.OptimizeResumeWithAI.V1;


public record OptimizeResumeWithAIRequestDto(string ResumeContent, string JobDescription);
public record OptimizeResumeWithAIResponseDto(Guid ResultId, string OptimizedResume);
