using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record Invoice : Entity<InvoiceId>
{
    public string InvoiceNumber { get; private set; } = default!;
    public Money Amount { get; private set; } = default!;
    public CurrencyCode Currency { get; private set; } = default!;
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public InvoiceStatus Status { get; private set; }

    private Invoice() { }

    public static Invoice Create(
        InvoiceId id,
        string invoiceNumber,
        Money amount,
        CurrencyCode currency)
    {
        return new Invoice
        {
            Id = id,
            InvoiceNumber = invoiceNumber,
            Amount = amount,
            Currency = currency,
            Status = InvoiceStatus.Unpaid,
            IssuedAt = DateTime.UtcNow
        };
    }

    public void ApplyDiscount(decimal percentage)
    {
        if (percentage <= 0 || percentage > 100) return;
        var discountAmount = Amount.Amount * (percentage / 100);
        Amount = Money.Of(Amount.Amount - discountAmount, Amount.Currency);
    }

    public void UpdateAmount(Money newAmount)
    {
        Amount = newAmount;
    }

    public void MarkPaid()
    {
        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = InvoiceStatus.Cancelled;
    }
}
