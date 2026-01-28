using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using Payment.Events;
using Payment.Features.CancelSubscription.V1;
using Payment.Features.CreateSubscription.V1;
using Payment.Features.GenerateInvoice.V1;
using Payment.Features.RecordUsageCharge.V1;

namespace Payment;

public sealed class PaymentEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            SubscriptionCreatedDomainEvent e => new AI.Contracts.EventBus.Messages.SubscriptionCreated(e.SubscriptionId.Value),
            InvoiceGeneratedDomainEvent e => new AI.Contracts.EventBus.Messages.InvoiceGenerated(e.InvoiceId.Value),
            UsageChargedDomainEvent e => new AI.Contracts.EventBus.Messages.UsageCharged(e.ChargeId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            SubscriptionCreatedDomainEvent e => new CreateSubscriptionMongo(e.SubscriptionId.Value, e.UserId.Value, e.Plan, e.Status, e.StartedAt, e.ExpiresAt),
            SubscriptionCancelledDomainEvent e => new CancelSubscriptionMongo(e.SubscriptionId.Value, e.Status),
            InvoiceGeneratedDomainEvent e => new GenerateInvoiceMongo(e.SubscriptionId.Value, e.InvoiceId.Value, e.Amount, e.Currency, e.Status, e.InvoiceNumber, e.IssuedAt),
            UsageChargedDomainEvent e => new RecordUsageChargeMongo(e.SubscriptionId.Value, e.ChargeId.Value, e.Amount, e.Currency, e.Module, e.Description, e.CreatedAt),
            _ => null
        };
    }
}