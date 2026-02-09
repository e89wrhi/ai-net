using CodeDebug.Enums;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public record StreamAnalyzeCodeRequestDto(Guid UserId, string Code, ProgrammingLanguage Language, DebugDepth Depth, bool IncludeSuggestion, string? ModelId = null);

