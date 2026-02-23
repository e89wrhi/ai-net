using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record UsageLimitReached(Guid Id) : IIntegrationEvent;
public record UserActivityAdded(Guid Id, Guid UserId, string Module, string Action) : IIntegrationEvent;
public record UserProfileUpdated(Guid Id) : IIntegrationEvent;
public record UserFeedbackSubmitted(Guid Id, string Feedback) : IIntegrationEvent;
public record UserPreferencesUpdated(Guid Id) : IIntegrationEvent;
public record UserDeviceRegistered(Guid Id, string DeviceType) : IIntegrationEvent;