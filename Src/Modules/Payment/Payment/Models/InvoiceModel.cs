using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record InvoiceModel : Entity<InvoiceId>
{
    public SubscriptionId SubscriptionId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public BillingPeriod Period { get; private set; } = default!;

    public Money TotalAmount { get; private set; } = default!;

    public string LineItems { get; private set; } = default!;

    public InvoiceStatus Status { get; private set; } = default!;
    public string InvoiceNumber { get; private set; } = default!;
    public DateTime IssueDate { get; private set; } = default!;
    public DateTime DueDate { get; private set; } = default!;
    public DateTime? PaidDate { get; private set; } = default!;
}