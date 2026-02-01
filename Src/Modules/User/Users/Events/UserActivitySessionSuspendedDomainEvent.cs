using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserActivitySessionSuspendedDomainEvent(UserActivityId Id) : IDomainEvent;
