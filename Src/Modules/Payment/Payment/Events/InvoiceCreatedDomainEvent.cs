using AI.Common.Core;
using Payment.Models;
using Payment.ValueObjects;

namespace Payment.Events;

public record InvoiceCreatedDomainEvent(PaymentId Id, InvoiceId InvoiceId, Money Amount) : IDomainEvent;
