using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeDebug.Enums;
using CodeDebug.ValueObjects;

namespace CodeDebug.Models;

public record CodeDebugReport : Entity<CodeDebugReportId>
{
    public SourceCode Code { get; private set; } = default!;
    public ProgrammingLanguage Language { get; private set; }
    public DebugSummary Summary { get; private set; } = default!;
    public IssueCount IssueCount { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime AnalyzedAt { get; private set; }

    private CodeDebugReport() { }

    public static CodeDebugReport Create(
        CodeDebugReportId id,
        SourceCode code,
        ProgrammingLanguage language,
        DebugSummary summary,
        IssueCount issueCount,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new CodeDebugReport
        {
            Id = id,
            Code = code,
            Language = language,
            Summary = summary,
            IssueCount = issueCount,
            TokenUsed = tokenUsed,
            Cost = cost,
            AnalyzedAt = DateTime.UtcNow
        };
    }
}
