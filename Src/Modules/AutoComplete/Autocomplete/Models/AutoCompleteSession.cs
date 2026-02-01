using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using AutoComplete.Enums;
using AutoComplete.ValueObjects;

namespace AutoComplete.Models;

public record AutoCompleteSession : Aggregate<AutoCompleteId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public AutoCompleteStatus Status { get; private set; }
    public AutoCompleteConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastRequestedAt { get; private set; }

    private readonly List<AutoCompleteRequest> _requests = new();
    public IReadOnlyCollection<AutoCompleteRequest> Requests => _requests.AsReadOnly();

    private AutoCompleteSession() { }

    public static AutoCompleteSession Create(
        AutoCompleteId id,
        UserId userId,
        ModelId aiModelId,
        AutoCompleteConfiguration configuration)
    {
        var session = new AutoCompleteSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = AutoCompleteStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastRequestedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.AutoCompleteSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddRequest(AutoCompleteRequest request)
    {
        if (Status != AutoCompleteStatus.Active)
            throw new DomainException("Autocomplete session is not active.");

        _requests.Add(request);
        LastRequestedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += request.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += request.Cost);
        AddDomainEvent(
            new Events.AutoCompleteRequestedDomainEvent(
                Id, request.Id, request.Prompt.Value, request.Suggestion.Value, request.TokenUsed.Value));
    }

    public void Complete()
    {
        Status = AutoCompleteStatus.Completed;
        AddDomainEvent(new Events.AutoCompleteSessionCompletedDomainEvent(Id));
    }

    public void Fail(AutoCompleteFailureReason reason)
    {
        Status = AutoCompleteStatus.Failed;
        AddDomainEvent(new Events.AutoCompleteSessionFailedDomainEvent(Id, reason));
    }
}
