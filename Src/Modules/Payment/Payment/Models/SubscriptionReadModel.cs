namespace Payment.Models;

public class SubscriptionReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<InvoiceReadModel> Invoices { get; set; } = new();
    public List<UsageChargeReadModel> Charges { get; set; } = new();
}

public class InvoiceReadModel
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string InvoiceNumber { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
}

public class UsageChargeReadModel
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string Module { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

