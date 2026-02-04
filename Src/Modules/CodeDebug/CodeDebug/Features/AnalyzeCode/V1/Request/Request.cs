using CodeDebug.Enums;

namespace CodeDebug.Features.AnalyzeCode.V1;

public record AnalyzeCodeRequestDto(string Code, ProgrammingLanguage Language, string? ModelId = null);
public record AnalyzeCodeResponseDto(Guid SessionId, Guid ReportId, string Summary, int IssueCount, string ModelId, string? ProviderName);

