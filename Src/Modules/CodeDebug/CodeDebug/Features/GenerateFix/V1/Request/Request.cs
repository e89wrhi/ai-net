namespace CodeDebug.Features.GenerateFix.V1;

public record GenerateFixRequestDto(Guid SessionId, Guid ReportId);
public record GenerateFixResponseDto(string FixedCode, string Explanation);
