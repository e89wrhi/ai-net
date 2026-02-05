using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record SubscriptionUpgradedDomainEvent(SubscriptionId Id, string NewPlanName) : IDomainEvent;
public record SubscriptionDowngradedDomainEvent(SubscriptionId Id, string NewPlanName) : IDomainEvent;
public record SubscriptionCancelledDomainEvent(SubscriptionId Id) : IDomainEvent;
public record SubscriptionPausedDomainEvent(SubscriptionId Id) : IDomainEvent;
public record SubscriptionResumedDomainEvent(SubscriptionId Id) : IDomainEvent;
public record PaymentRefundedDomainEvent(PaymentId Id) : IDomainEvent;
