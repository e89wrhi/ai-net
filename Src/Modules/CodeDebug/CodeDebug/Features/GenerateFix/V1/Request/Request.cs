namespace CodeDebug.Features.GenerateFix.V1;

public record GenerateFixRequestDto(Guid SessionId, Guid ReportId, string? ModelId = null);
public record GenerateFixResponseDto(string FixedCode, string Explanation, string ModelId, string? ProviderName);
