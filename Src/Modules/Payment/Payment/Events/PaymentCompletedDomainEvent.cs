using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record PaymentCompletedDomainEvent(PaymentId Id) : IDomainEvent;
