using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageGen.Enums;
using ImageGen.ValueObjects;

namespace ImageGen.Models;

public record ImageGenerationSession : Aggregate<ImageGenId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public ImageGenerationStatus Status { get; private set; }
    public ImageGenerationConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastGeneratedAt { get; private set; }

    private readonly List<ImageGenerationResult> _results = new();
    public IReadOnlyCollection<ImageGenerationResult> Results => _results.AsReadOnly();

    private ImageGenerationSession() { }

    public static ImageGenerationSession Create(
        ImageGenId id,
        UserId userId,
        ModelId aiModelId,
        ImageGenerationConfiguration configuration)
    {
        var session = new ImageGenerationSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = ImageGenerationStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastGeneratedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.ImageGenerationSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(ImageGenerationResult result)
    {
        if (Status != ImageGenerationStatus.Active)
            throw new DomainException("Image generation session is not active.");

        _results.Add(result);
        LastGeneratedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);
        AddDomainEvent(
            new Events.ImageGeneratedDomainEvent(
                Id, result.Id));
    }

    public void Complete()
    {
        Status = ImageGenerationStatus.Completed;
        AddDomainEvent(new Events.ImageGenerationSessionCompletedDomainEvent(Id));
    }

    public void Fail(ImageGenerationFailureReason reason)
    {
        Status = ImageGenerationStatus.Failed;
        AddDomainEvent(new Events.ImageGenerationSessionFailedDomainEvent(Id, reason));
    }
}
