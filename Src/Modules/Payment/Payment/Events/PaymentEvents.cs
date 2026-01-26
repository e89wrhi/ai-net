using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record SubscriptionCreatedDomainEvent(SubscriptionId SubscriptionId, UserId UserId, string Plan) : IDomainEvent;
public record SubscriptionRenewedDomainEvent(SubscriptionId SubscriptionId, DateTime NextExpiry) : IDomainEvent;
public record InvoiceGeneratedDomainEvent(InvoiceId InvoiceId, SubscriptionId SubscriptionId, decimal Amount) : IDomainEvent;
public record UsageChargedDomainEvent(SubscriptionId SubscriptionId, string Reason, decimal Amount) : IDomainEvent;
public record PaymentFailedDomainEvent(SubscriptionId SubscriptionId, string Reason) : IDomainEvent;
