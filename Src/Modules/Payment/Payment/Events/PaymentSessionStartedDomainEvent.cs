using AI.Common.Core;
using Bogus.DataSets;
using Payment.ValueObjects;

namespace Payment.Events;

public record PaymentSessionStartedDomainEvent(PaymentId Id, UserId UserId, Money Amount, string Currency) : IDomainEvent;
