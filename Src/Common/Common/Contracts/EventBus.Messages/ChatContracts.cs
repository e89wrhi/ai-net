using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record ChatSessionStarted(Guid Id) : IIntegrationEvent;
public record ChatSessionEnded(Guid Id) : IIntegrationEvent;
public record MessageRecieved(Guid Id) : IIntegrationEvent;
public record MessageResponded(Guid Id) : IIntegrationEvent;
public record MessageDeleted(Guid Id) : IIntegrationEvent;
public record MessageEdited(Guid Id) : IIntegrationEvent;
public record TypingStarted(Guid Id, Guid UserId) : IIntegrationEvent;