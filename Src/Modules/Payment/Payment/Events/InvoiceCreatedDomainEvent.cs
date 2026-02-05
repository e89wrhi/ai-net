using AI.Common.Core;
using Payment.Models;
using Payment.ValueObjects;

namespace Payment.Events;

public record InvoiceCreatedDomainEvent(Guid OwnerId, InvoiceId InvoiceId, Money Amount) : IDomainEvent;
