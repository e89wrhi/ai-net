using AI.Common.Core;
using User.ValueObjects;
using User.Enums;

namespace User.Events;

public record UserActivityTrackedDomainEvent(UserId UserId, UserActivityId ActivityId, TrackedModule Module, string Action) : IDomainEvent;
public record UsageRecordAddedDomainEvent(UserId UserId, UsageContainerId UsageId, string TokenUsed) : IDomainEvent;
