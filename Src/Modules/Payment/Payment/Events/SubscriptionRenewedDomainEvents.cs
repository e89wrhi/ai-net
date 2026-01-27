using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record SubscriptionRenewedDomainEvent(SubscriptionId SubscriptionId, DateTime NextExpiry) : IDomainEvent;
