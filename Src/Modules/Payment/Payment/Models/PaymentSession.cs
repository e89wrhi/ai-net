using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record PaymentSession : Aggregate<PaymentId>
{
    public UserId UserId { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; } = default!;
    public Money Amount { get; private set; } = default!;
    public CurrencyCode Currency { get; private set; } = default!;
    public DateTime LastUpdatedAt { get; private set; }

    private readonly List<Invoice> _invoices = new();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    private PaymentSession() { }

    public static PaymentSession Create(
        PaymentId id,
        UserId userId,
        PaymentMethod paymentMethod,
        Money amount,
        CurrencyCode currency)
    {
        var session = new PaymentSession
        {
            Id = id,
            UserId = userId,
            PaymentMethod = paymentMethod,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.PaymentSessionStartedDomainEvent(id, userId, amount, currency));

        return session;
    }

    public void AddInvoice(Invoice invoice)
    {
        _invoices.Add(invoice);
        LastUpdatedAt = DateTime.UtcNow;

        AddDomainEvent(
            new Events.InvoiceCreatedDomainEvent(Id, invoice.Id, invoice.Amount));
    }

    public void Complete()
    {
        Status = PaymentStatus.Completed;
        LastUpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new Events.PaymentCompletedDomainEvent(Id));
    }

    public void Fail(PaymentFailureReason reason)
    {
        Status = PaymentStatus.Failed;
        LastUpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new Events.PaymentFailedDomainEvent(Id, reason));
    }
}
