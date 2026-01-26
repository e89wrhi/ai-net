using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UsageLimitReachedDomainEvent(UserId UserId, string Message) : IDomainEvent;
