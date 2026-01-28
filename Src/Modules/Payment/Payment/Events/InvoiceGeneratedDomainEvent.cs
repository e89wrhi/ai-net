using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record InvoiceGeneratedDomainEvent(InvoiceId InvoiceId, SubscriptionId SubscriptionId, decimal Amount, string Currency, string Status, string InvoiceNumber, DateTime IssuedAt) : IDomainEvent;
