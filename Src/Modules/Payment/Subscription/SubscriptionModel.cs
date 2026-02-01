using AI.Common.Core;
using Payment.Enums;
using Payment.ValueObjects;

namespace Payment.Models;

public record SubscriptionModel : Aggregate<SubscriptionId>
{
    public UserId UserId { get; private set; } = default!;
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndsAt { get; private set; }
    
    // Limits
    public int MaxRequestsPerDay { get; private set; }
    public int MaxTokensPerMonth { get; private set; }
    
    // Plan details (Simplified or could reference a Plan aggregate)
    public string PlanName { get; private set; } = default!;

    private readonly List<Invoice> _invoices = new();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    private readonly List<UsageCharge> _charges = new();
    public IReadOnlyCollection<UsageCharge> Charges => _charges.AsReadOnly();

    private SubscriptionModel() { }

    public void AddInvoice(Invoice invoice)
    {
        _invoices.Add(invoice);
        AddDomainEvent(new Events.InvoiceCreatedDomainEvent(Id, invoice.Id, invoice.Amount));
    }

    public void AddCharge(UsageCharge charge)
    {
        _charges.Add(charge);
    }

    public static SubscriptionModel Create(
        SubscriptionId id, 
        UserId userId, 
        string planName, 
        int maxRequestsPerDay, 
        int maxTokensPerMonth)
    {
        return new SubscriptionModel
        {
            Id = id,
            UserId = userId,
            PlanName = planName,
            MaxRequestsPerDay = maxRequestsPerDay,
            MaxTokensPerMonth = maxTokensPerMonth,
            Status = SubscriptionStatus.Active,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Upgrade(string newPlanName, int newMaxRequests, int newMaxTokens)
    {
        PlanName = newPlanName;
        MaxRequestsPerDay = newMaxRequests;
        MaxTokensPerMonth = newMaxTokens;
        
        AddDomainEvent(new Events.SubscriptionUpgradedDomainEvent(Id, newPlanName));
    }

    public void Downgrade(string newPlanName, int newMaxRequests, int newMaxTokens)
    {
        PlanName = newPlanName;
        MaxRequestsPerDay = newMaxRequests;
        MaxTokensPerMonth = newMaxTokens;
        
        AddDomainEvent(new Events.SubscriptionDowngradedDomainEvent(Id, newPlanName));
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        EndsAt = DateTime.UtcNow;
        AddDomainEvent(new Events.SubscriptionCancelledDomainEvent(Id));
    }

    public void Pause()
    {
        Status = SubscriptionStatus.Paused;
        AddDomainEvent(new Events.SubscriptionPausedDomainEvent(Id));
    }

    public void Resume()
    {
        Status = SubscriptionStatus.Active;
        AddDomainEvent(new Events.SubscriptionResumedDomainEvent(Id));
    }
}
