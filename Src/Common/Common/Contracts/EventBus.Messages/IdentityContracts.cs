using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record UserCreated(Guid Id, string Name, string PassportNumber) : IIntegrationEvent;
public record UserUpdated(Guid Id) : IIntegrationEvent;
public record UserDeleted(Guid Id) : IIntegrationEvent;
public record UserLocked(Guid Id, string Reason) : IIntegrationEvent;
public record UserUnlocked(Guid Id) : IIntegrationEvent;
public record UserLoggedIn(Guid Id, Guid UserId) : IIntegrationEvent;
public record UserLoggedOut(Guid Id, Guid UserId) : IIntegrationEvent;