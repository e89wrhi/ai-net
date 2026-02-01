using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageCaption.Enums;
using ImageCaption.ValueObjects;

namespace ImageCaption.Models;

public record ImageCaptionSession : Aggregate<ImageCaptionId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public ImageCaptionStatus Status { get; private set; }
    public ImageCaptionConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastCaptionedAt { get; private set; }

    private readonly List<ImageCaptionResult> _results = new();
    public IReadOnlyCollection<ImageCaptionResult> Results => _results.AsReadOnly();

    private ImageCaptionSession() { }

    public static ImageCaptionSession Create(
        ImageCaptionId id,
        UserId userId,
        ModelId aiModelId,
        ImageCaptionConfiguration configuration)
    {
        var session = new ImageCaptionSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = ImageCaptionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastCaptionedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.ImageCaptionSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(ImageCaptionResult result)
    {
        if (Status != ImageCaptionStatus.Active)
            throw new DomainException("Image caption session is not active.");

        _results.Add(result);
        LastCaptionedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.ImageCaptionGeneratedDomainEvent(
                Id, result.Id, result.Caption.Value));
    }

    public void Complete()
    {
        Status = ImageCaptionStatus.Completed;
        AddDomainEvent(new Events.ImageCaptionSessionCompletedDomainEvent(Id));
    }

    public void Fail(ImageCaptionFailureReason reason)
    {
        Status = ImageCaptionStatus.Failed;
        AddDomainEvent(new Events.ImageCaptionSessionFailedDomainEvent(Id, reason));
    }
}
