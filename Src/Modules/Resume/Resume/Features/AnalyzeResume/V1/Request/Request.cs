namespace Resume.Features.AnalyzeResume.V1;

public record AnalyzeResumeWithAIRequestDto(string ResumeContent);
public record AnalyzeResumeWithAIResponseDto(Guid SessionId, Guid ResultId, string Summary, double Score);
