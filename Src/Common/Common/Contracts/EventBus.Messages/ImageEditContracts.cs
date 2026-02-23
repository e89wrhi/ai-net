using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record ImageEdited(Guid Id, string Operation) : IIntegrationEvent;
public record FilterApplied(Guid Id, string FilterName) : IIntegrationEvent;
public record ImageUpscaled(Guid Id, double ScaleFactor) : IIntegrationEvent;
public record BackgroundRemoved(Guid Id) : IIntegrationEvent;
