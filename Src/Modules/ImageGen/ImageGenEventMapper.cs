using AI.Common.Contracts.EventBus.ImageGens;
using AI.Common.Core;
using ImageGen.Events;
using ImageGen.Features.DeleteImageGen.V1;
using ImageGen.Features.SendImageGen.V1;
using ImageGen.Features.StartImageGen.V1;

namespace ImageGen;

public sealed class ImageGenEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ImageGenerationSessionStartedDomainEvent e => new ImageGenSessionStarted(e.SessionId.Value),
            ImageGeneratedDomainEvent e => new ImageGenRecieved(e.ImageGenId.Value),
            ImageGenRespondedDomainEvent e => new ImageGenResponded(e.ImageGenId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ImageGenerationSessionStartedDomainEvent e => new StartImageGenMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            ImageGeneratedDomainEvent e => new SendImageGenMongo(e.SessionId.Value, e.ImageGenId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            ImageGenRespondedDomainEvent e => new SendImageGenMongo(e.SessionId.Value, e.ImageGenId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            ImageGenSessionDeletedDomainEvent e => new GenerateImageMongo(e.Id.Value),
            _ => null
        };
    }
}