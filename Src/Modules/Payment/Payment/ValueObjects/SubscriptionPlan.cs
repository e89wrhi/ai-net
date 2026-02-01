using Payment.Enums;
using Payment.Exceptions;

namespace Payment.ValueObjects;

public record SubscriptionPlan
{
    public string Name { get; }
    public Money Price { get; }
    public BillingCycle Cycle { get; }

    public SubscriptionPlan(string name, Money price, BillingCycle cycle)
    {
        Name = name;
        Price = price;
        Cycle = cycle;
    }
}
