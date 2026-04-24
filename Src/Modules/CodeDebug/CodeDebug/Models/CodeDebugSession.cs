using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeDebug.Enums;
using CodeDebug.ValueObjects;

namespace CodeDebug.Models;

public record CodeDebugSession : Aggregate<CodeDebugId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public CodeDebugStatus Status { get; private set; }
    public CodeDebugConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastAnalyzedAt { get; private set; }

    private readonly List<CodeDebugReport> _reports = new();
    public IReadOnlyCollection<CodeDebugReport> Reports => _reports.AsReadOnly();

    private CodeDebugSession() 
    { 
        _reports = new();
    }

    public static CodeDebugSession Create(
        CodeDebugId id,
        UserId userId,
        ModelId aiModelId,
        CodeDebugConfiguration configuration)
    {
        var session = new CodeDebugSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = CodeDebugStatus.Active,
            TotalTokens = TokenCount.Of(0),
            TotalCost = CostEstimate.Of(0),
            CreatedAt = DateTime.UtcNow,
            LastAnalyzedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.CodeDebugSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddReport(CodeDebugReport report)
    {
        if (Status != CodeDebugStatus.Active)
            throw new DomainException("Code debug session is not active.");

        _reports.Add(report);
        LastAnalyzedAt = DateTime.UtcNow;
        
        TotalTokens = TokenCount.Of((long)TotalTokens + (long)report.TokenUsed);
        TotalCost = CostEstimate.Of((decimal)TotalCost + (decimal)report.Cost);
        
        AddDomainEvent(
            new Events.CodeDebugAnalyzedDomainEvent(
                Id, report.Id, report.IssueCount));
    }

    public void Complete()
    {
        Status = CodeDebugStatus.Completed;
        AddDomainEvent(new Events.CodeDebugSessionCompletedDomainEvent(Id));
    }

    public void Fail(CodeDebugFailureReason reason)
    {
        Status = CodeDebugStatus.Failed;
        AddDomainEvent(new Events.CodeDebugSessionFailedDomainEvent(Id, reason));
    }
}
