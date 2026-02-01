using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserActivitySessionStartedDomainEvent(UserActivityId Id, UserId UserId) : IDomainEvent;
