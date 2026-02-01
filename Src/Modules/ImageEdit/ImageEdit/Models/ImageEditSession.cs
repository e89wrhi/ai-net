using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageEdit.Enums;
using ImageEdit.ValueObjects;

namespace ImageEdit.Models;

public record ImageEditSession : Aggregate<ImageEditId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public ImageEditStatus Status { get; private set; }
    public ImageEditConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastEditedAt { get; private set; }

    private readonly List<ImageEditResult> _results = new();
    public IReadOnlyCollection<ImageEditResult> Results => _results.AsReadOnly();

    private ImageEditSession() { }

    public static ImageEditSession Create(
        ImageEditId id,
        UserId userId,
        ModelId aiModelId,
        ImageEditConfiguration configuration)
    {
        var session = new ImageEditSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = ImageEditStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastEditedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.ImageEditSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddResult(ImageEditResult result)
    {
        if (Status != ImageEditStatus.Active)
            throw new DomainException("Image edit session is not active.");

        _results.Add(result);
        LastEditedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += result.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += result.Cost);

        AddDomainEvent(
            new Events.ImageEditedDomainEvent(
                Id, result.Id));
    }

    public void Complete()
    {
        Status = ImageEditStatus.Completed;
        AddDomainEvent(new Events.ImageEditSessionCompletedDomainEvent(Id));
    }

    public void Fail(ImageEditFailureReason reason)
    {
        Status = ImageEditStatus.Failed;
        AddDomainEvent(new Events.ImageEditSessionFailedDomainEvent(Id, reason));
    }
}

