namespace Payment.Dtos;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string InvoiceNumber { get; set; } = default!;
    public DateTime IssueDate { get; set; }
}

