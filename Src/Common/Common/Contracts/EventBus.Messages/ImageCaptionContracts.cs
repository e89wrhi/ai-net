using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record ImageUploaded(Guid Id) : IIntegrationEvent;
public record ImageAnalyzed(Guid Id) : IIntegrationEvent;
public record ImageCaptionGenerated(Guid Id) : IIntegrationEvent;
public record ImageDescriptionRefined(Guid Id) : IIntegrationEvent;
public record ImageTagsExtracted(Guid Id, string[] Tags) : IIntegrationEvent;
public record ImageSafetyCheckCompleted(Guid Id, bool IsSafe) : IIntegrationEvent;