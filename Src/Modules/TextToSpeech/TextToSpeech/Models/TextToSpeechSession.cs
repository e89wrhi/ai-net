using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using TextToSpeech.Enums;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Models;

public record TextToSpeechSession : Aggregate<TextToSpeechId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public TextToSpeechStatus Status { get; private set; }
    public TextToSpeechConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastSynthesizedAt { get; private set; }

    private readonly List<TextToSpeechResult> _results = new();
    public IReadOnlyCollection<TextToSpeechResult> Results => _results.AsReadOnly();

    private TextToSpeechSession() { }

    public static TextToSpeechSession Create(
        TextToSpeechId id,
        UserId userId,
        ModelId aiModelId,
        TextToSpeechConfiguration configuration)
    {
        var session = new TextToSpeechSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = TextToSpeechStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastSynthesizedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.TextToSpeechSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(TextToSpeechResult result)
    {
        if (Status != TextToSpeechStatus.Active)
            throw new DomainException("Text-to-speech session is not active.");

        _results.Add(result);
        LastSynthesizedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.TextSynthesizedDomainEvent(
                Id, result.Id, result.Speech.Value));
    }

    public void Complete()
    {
        Status = TextToSpeechStatus.Completed;
        AddDomainEvent(new Events.TextToSpeechSessionCompletedDomainEvent(Id));
    }

    public void Fail(TextToSpeechFailureReason reason)
    {
        Status = TextToSpeechStatus.Failed;
        AddDomainEvent(new Events.TextToSpeechSessionFailedDomainEvent(Id, reason));
    }
}
