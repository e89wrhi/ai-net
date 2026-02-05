using Payment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.ValueObjects;

namespace Payment.Data.Configurations;

public class UsageChargeConfiguration : IEntityTypeConfiguration<UsageCharge>
{
    public void Configure(EntityTypeBuilder<UsageCharge> builder)
    {
        builder.ToTable("usage_charges");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => PaymentId.Of(dbId));

        builder.Property(r => r.SubscriptionId)
            .HasConversion(id => id.Value, value => SubscriptionId.Of(value));

        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.TokenUsed);
        builder.Property(r => r.Description);
        builder.Property(r => r.Module);

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(m => m.Amount).HasColumnName("Amount");
             b.Property(m => m.Currency).HasColumnName("Currency");
        });

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
