using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record PaymentFailedDomainEvent(SubscriptionId SubscriptionId, string Reason) : IDomainEvent;
