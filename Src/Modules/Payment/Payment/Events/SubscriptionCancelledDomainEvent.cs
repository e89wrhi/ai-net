using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record SubscriptionCancelledDomainEvent(SubscriptionId SubscriptionId, string Status) : IDomainEvent;
