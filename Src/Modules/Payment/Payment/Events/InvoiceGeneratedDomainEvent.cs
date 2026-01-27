using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record InvoiceGeneratedDomainEvent(InvoiceId InvoiceId, SubscriptionId SubscriptionId, decimal Amount) : IDomainEvent;
