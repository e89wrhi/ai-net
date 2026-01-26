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

}