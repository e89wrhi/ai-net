using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record ImageUploaded(Guid Id) : IIntegrationEvent;
public record ImageAnalyzed(Guid Id) : IIntegrationEvent;
public record ImageCaptionGenerated(Guid Id) : IIntegrationEvent;