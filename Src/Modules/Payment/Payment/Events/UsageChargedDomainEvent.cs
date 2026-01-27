using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record UsageChargedDomainEvent(SubscriptionId SubscriptionId, string Reason, decimal Amount) : IDomainEvent;
