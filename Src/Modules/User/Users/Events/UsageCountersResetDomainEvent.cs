using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UsageCountersResetDomainEvent(UserId UserId) : IDomainEvent;
