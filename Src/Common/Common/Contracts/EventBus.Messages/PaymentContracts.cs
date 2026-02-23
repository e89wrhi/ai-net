using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record InvoiceGenerated(Guid Id) : IIntegrationEvent;
public record PaymentSucceeded(Guid Id) : IIntegrationEvent;
public record PaymentFailed(Guid Id) : IIntegrationEvent;
public record SubscriptionCreated(Guid Id) : IIntegrationEvent;
public record SubscriptionRenewed(Guid Id) : IIntegrationEvent;
public record SubscriptionCancelled(Guid Id) : IIntegrationEvent;
public record UsageCharged(Guid Id) : IIntegrationEvent;
public record RefundProcessed(Guid Id, decimal Amount) : IIntegrationEvent;
public record PromoCodeApplied(Guid Id, string Code) : IIntegrationEvent;