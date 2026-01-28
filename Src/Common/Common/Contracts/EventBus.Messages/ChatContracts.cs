using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record ChatSessionStarted(Guid Id) : IIntegrationEvent;
public record MessageRecieved(Guid Id) : IIntegrationEvent;
public record MessageResponded(Guid Id) : IIntegrationEvent;