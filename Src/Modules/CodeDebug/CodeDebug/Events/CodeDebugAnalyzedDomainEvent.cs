using AI.Common.Core;
using CodeDebug.ValueObjects;

namespace CodeDebug.Events;

public record CodeDebugAnalyzedDomainEvent(CodeDebugId Id,CodeDebugReportId ReportId, int IssueCount) : IDomainEvent;
