using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record InvoiceGenerated(Guid Id) : IIntegrationEvent;
public record PaymentFailed(Guid Id) : IIntegrationEvent;
public record SubscriptionCreated(Guid Id) : IIntegrationEvent;
public record SubscriptionRenewed(Guid Id) : IIntegrationEvent;
public record UsageCharged(Guid Id) : IIntegrationEvent;