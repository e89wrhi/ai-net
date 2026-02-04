namespace Resume.Features.AnalyzeResume.V1;

public record AnalyzeResumeRequestDto(string ResumeContent, bool IncludeSkill, bool IncludeEducation, bool IncludeExpireance, string? ModelId = null);
public record AnalyzeResumeResponseDto(Guid SessionId, Guid ResultId, string Summary, double Score, string ModelId, string? ProviderName);
