using AI.Common.Core;

namespace CodeDebug.Features.AnalyzeCode.V1;

public record AnalyzeCodeCommand(string Code, ProgrammingLanguage Language) : ICommand<AnalyzeCodeCommandResult>;

public record AnalyzeCodeCommandResult(Guid SessionId, Guid ReportId, string Summary, int IssueCount);

