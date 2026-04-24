using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeGen.Enums;
using CodeGen.ValueObjects;

namespace CodeGen.Models;

public record CodeGenerationSession : Aggregate<CodeGenId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public CodeGenerationStatus Status { get; private set; }
    public CodeGenerationConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastGeneratedAt { get; private set; }

    private readonly List<CodeGenerationResult> _results = new();
    public IReadOnlyCollection<CodeGenerationResult> Results => _results.AsReadOnly();

    private CodeGenerationSession() 
    {
        _results = new();
    }

    public static CodeGenerationSession Create(
        CodeGenId id,
        UserId userId,
        ModelId aiModelId,
        CodeGenerationConfiguration configuration)
    {
        var session = new CodeGenerationSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = CodeGenerationStatus.Active,
            TotalTokens = TokenCount.Of(0),
            TotalCost = CostEstimate.Of(0),
            CreatedAt = DateTime.UtcNow,
            LastGeneratedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.CodeGenerationSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(CodeGenerationResult result)
    {
        if (Status != CodeGenerationStatus.Active)
            throw new DomainException("Code generation session is not active.");

        _results.Add(result);
        LastGeneratedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);
        AddDomainEvent(
            new Events.CodeGeneratedDomainEvent(
                Id, result.Id, result.Language));
    }

    public void Complete()
    {
        Status = CodeGenerationStatus.Completed;
        AddDomainEvent(new Events.CodeGenerationSessionCompletedDomainEvent(Id));
    }

    public void Fail(CodeGenerationFailureReason reason)
    {
        Status = CodeGenerationStatus.Failed;
        AddDomainEvent(new Events.CodeGenerationSessionFailedDomainEvent(Id, reason));
    }
}

