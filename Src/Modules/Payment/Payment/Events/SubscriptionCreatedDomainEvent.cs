using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Events;

public record SubscriptionCreatedDomainEvent(SubscriptionId SubscriptionId, UserId UserId, string Plan) : IDomainEvent;
