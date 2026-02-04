namespace Resume.Features.OptimizeResume.V1;

public record OptimizeResumeRequestDto(string ResumeContent, string JobDescription, string? ModelId = null);
public record OptimizeResumeResponseDto(Guid ResultId, string OptimizedResume, string ModelId, string? ProviderName);
