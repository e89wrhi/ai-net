using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record UsageLimitReached(Guid Id) : IIntegrationEvent;
public record UserActivityAdded(Guid Id, Guid UserId, string Module, string Action) : IIntegrationEvent;
public record UserProfileUpdated(Guid Id) : IIntegrationEvent;