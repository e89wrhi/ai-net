using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using ImageCaption.Events;
using ImageCaption.Features.GenerateCaption.V1;
using ImageCaption.Features.UploadImage.V1;

namespace ImageCaption;

public sealed class ImageEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ImageUploadedDomainEvent e => new ImageUploaded(e.ImageId.Value),
            ImageAnalyzedDomainEvent e => new ImageAnalyzed(e.ImageId.Value),
            CaptionGeneratedDomainEvent e => new ImageCaptionGenerated(e.ImageId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ImageUploadedDomainEvent e => new UploadImageMongo(e.ImageId.Value, e.UserId, e.FilePath, "Uploaded", e.Width, e.Height, e.Size, e.Format, DateTime.UtcNow),
            CaptionGeneratedDomainEvent e => new GenerateCaptionMongo(e.ImageId.Value, e.Caption, "Captioned", DateTime.UtcNow),
            _ => null
        };
    }
}