using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserActivitySessionCompletedDomainEvent(UserActivityId Id) : IDomainEvent;
