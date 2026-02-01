using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Events;

public record PaymentFailedDomainEvent(PaymentId Id, PaymentFailureReason Reason) : IDomainEvent;
