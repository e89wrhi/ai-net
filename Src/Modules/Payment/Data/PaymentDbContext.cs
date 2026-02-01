using Payment.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Payment.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class PaymentDbContext : AppDbContextBase
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<PaymentDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<PaymentSession> Sessions => Set<PaymentSession>();
    public DbSet<SubscriptionModel> Subscriptions => Set<SubscriptionModel>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<UsageCharge> UsageCharges => Set<UsageCharge>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}