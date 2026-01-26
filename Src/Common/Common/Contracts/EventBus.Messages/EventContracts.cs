using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record EventCreated(Guid Id) : IIntegrationEvent;
public record EventDeleted(Guid Id) : IIntegrationEvent;