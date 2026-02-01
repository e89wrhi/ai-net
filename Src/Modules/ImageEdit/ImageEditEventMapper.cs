using AI.Common.Contracts.EventBus.ImageEdits;
using AI.Common.Core;
using ImageEdit.Events;
using ImageEdit.Features.DeleteImageEdit.V1;
using ImageEdit.Features.SendImageEdit.V1;
using ImageEdit.Features.StartImageEdit.V1;

namespace ImageEdit;

public sealed class ImageEditEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ImageEditSessionCompletedDomainEvent e => new ImageEditSessionStarted(e.SessionId.Value),
            ImageEditSessionStartedDomainEvent e => new ImageEditRecieved(e.ImageEditId.Value),
            ImageEditedDomainEvent e => new ImageEditResponded(e.ImageEditId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ImageEditSessionCompletedDomainEvent e => new StartImageEditMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            ImageEditSessionStartedDomainEvent e => new SendImageEditMongo(e.SessionId.Value, e.ImageEditId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            ImageEditedDomainEvent e => new SendImageEditMongo(e.SessionId.Value, e.ImageEditId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            ImageEditSessionDeletedDomainEvent e => new EditImageMongo(e.Id.Value),
            _ => null
        };
    }
}