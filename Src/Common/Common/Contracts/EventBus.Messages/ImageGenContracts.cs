using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record ImageGenerated(Guid Id, string Prompt, string Provider) : IIntegrationEvent;
public record ImageBatchGenerated(Guid Id, int Count) : IIntegrationEvent;
public record StyleTransferred(Guid Id, string Style) : IIntegrationEvent;
public record ModelMetadataUpdated(Guid Id, string ModelName) : IIntegrationEvent;
