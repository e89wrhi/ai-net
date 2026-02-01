using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Resume.Enums;
using Resume.ValueObjects;
using System.Linq;

namespace Resume.Models;

public record ResumeAnalysisSession : Aggregate<ResumeId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public ResumeAnalysisStatus Status { get; private set; }
    public ResumeAnalysisConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastAnalyzedAt { get; private set; }

    private readonly List<ResumeAnalysisResult> _results = new();
    public IReadOnlyCollection<ResumeAnalysisResult> Results => _results.AsReadOnly();

    private ResumeAnalysisSession() { }

    public static ResumeAnalysisSession Create(
        ResumeId id,
        UserId userId,
        ModelId aiModelId,
        ResumeAnalysisConfiguration configuration)
    {
        var session = new ResumeAnalysisSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = ResumeAnalysisStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastAnalyzedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.ResumeAnalysisSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(ResumeAnalysisResult result)
    {
        if (Status != ResumeAnalysisStatus.Active)
            throw new DomainException("Resume analysis session is not active.");

        _results.Add(result);
        LastAnalyzedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);
        AddDomainEvent(
            new Events.ResumeAnalyzedDomainEvent(
                Id, result.Id, result.Summary.Value));
    }

    public void Complete()
    {
        Status = ResumeAnalysisStatus.Completed;
        AddDomainEvent(new Events.ResumeAnalysisSessionCompletedDomainEvent(Id));
    }

    public void Fail(ResumeAnalysisFailureReason reason)
    {
        Status = ResumeAnalysisStatus.Failed;
        AddDomainEvent(new Events.ResumeAnalysisSessionFailedDomainEvent(Id, reason));
    }
}
