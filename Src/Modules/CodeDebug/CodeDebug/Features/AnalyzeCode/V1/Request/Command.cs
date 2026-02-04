using AI.Common.Core;
using CodeDebug.Enums;

namespace CodeDebug.Features.AnalyzeCode.V1;

public record AnalyzeCodeCommand(string Code, ProgrammingLanguage Language, string? ModelId = null) : ICommand<AnalyzeCodeCommandResult>;

public record AnalyzeCodeCommandResult(Guid SessionId, Guid ReportId, string Summary, int IssueCount, string ModelId, string? ProviderName);
