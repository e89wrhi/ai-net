using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record SubscriptionModel : Aggregate<SubscriptionId>
{
    public UserId UserId { get; private set; } = default!;

    public PlanLimits PlanLimits { get; private set; } = default!;

    public SubscriptionPlan Plan { get; private set; } = default!;

    public SubscriptionStatus SubscriptionStatus { get; private set; } = default!;
    public bool CancelAtPeriodEnd { get; private set; } = default!;
    public string PaymentMethodId { get; private set; } = default!;
    public string Currency { get; private set; } = default!;

    public DateTime? StartedAt { get; private set; } = default!;

    public DateTime? RenewedAt { get; private set; } = default!;

    public DateTime? ExpiresAt { get; private set; } = default!;

    private readonly List<InvoiceModel> _invoices = new();
    public IReadOnlyCollection<InvoiceModel> Invoices => _invoices.AsReadOnly();


    private readonly List<UsageCharge> _charges = new();
    public IReadOnlyCollection<UsageCharge> Charges => _charges.AsReadOnly();

    private SubscriptionModel() { }

    public static SubscriptionModel Create(SubscriptionId id, UserId userId, SubscriptionPlan plan, PlanLimits limits)
    {
        var subscription = new SubscriptionModel
        {
            Id = id,
            UserId = userId,
            Plan = plan,
            PlanLimits = limits,
            SubscriptionStatus = SubscriptionStatus.Active,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        subscription.AddDomainEvent(new Payment.Events.SubscriptionCreatedDomainEvent(id, userId, plan.ToString(), SubscriptionStatus.Active.ToString(), subscription.StartedAt ?? DateTime.UtcNow, subscription.ExpiresAt ?? DateTime.UtcNow.AddMonths(1)));
        return subscription;
    }

    public void Renew(DateTime nextExpiry)
    {
        RenewedAt = DateTime.UtcNow;
        ExpiresAt = nextExpiry;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new Payment.Events.SubscriptionRenewedDomainEvent(Id, nextExpiry));
    }

    public void AddInvoice(InvoiceModel invoice)
    {
        _invoices.Add(invoice);
        AddDomainEvent(new Payment.Events.InvoiceGeneratedDomainEvent(invoice.Id, Id, invoice.TotalAmount, invoice.Currency, invoice.PaymentStatus.ToString(), invoice.InvoiceNumber, invoice.InvoiceDate));
    }

    public void AddCharge(UsageCharge charge)
    {
        _charges.Add(charge);
        AddDomainEvent(new Payment.Events.UsageChargedDomainEvent(Id, charge.Id, charge.Cost, charge.Currency, charge.Module.ToString(), charge.Description, charge.CreatedAt));
    }
    public void Cancel()
    {
        SubscriptionStatus = SubscriptionStatus.Cancelled;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new Payment.Events.SubscriptionCancelledDomainEvent(Id, SubscriptionStatus.Cancelled.ToString()));
    }

}
