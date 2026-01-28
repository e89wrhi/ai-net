using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record UsageChargedDomainEvent(SubscriptionId SubscriptionId, UsageChargeId ChargeId, decimal Amount, string Currency, string Module, string Description, DateTime CreatedAt) : IDomainEvent;
