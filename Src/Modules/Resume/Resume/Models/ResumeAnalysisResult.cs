using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Resume.ValueObjects;

namespace Resume.Models;

public record ResumeAnalysisResult : Entity<ResultId>
{
    public ResumeFile Resume { get; private set; } = default!;
    public ResumeSummary Summary { get; private set; } = default!;
    public CandidateScore Score { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime AnalyzedAt { get; private set; }

    private ResumeAnalysisResult() { }

    public static ResumeAnalysisResult Create(
        ResultId id,
        ResumeFile resume,
        ResumeSummary summary,
        CandidateScore score,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new ResumeAnalysisResult
        {
            Id = id,
            Resume = resume,
            Summary = summary,
            Score = score,
            TokenUsed = tokenUsed,
            Cost = cost,
            AnalyzedAt = DateTime.UtcNow
        };
    }
}
