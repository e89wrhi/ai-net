using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserActionRecordedDomainEvent(UserActivityId Id, UserActionId ActionId, string Action): IDomainEvent;