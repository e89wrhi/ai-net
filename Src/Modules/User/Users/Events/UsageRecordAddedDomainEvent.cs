using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UsageRecordAddedDomainEvent(UserId UserId, UsageContainerId UsageId, string TokenUsed) : IDomainEvent;
