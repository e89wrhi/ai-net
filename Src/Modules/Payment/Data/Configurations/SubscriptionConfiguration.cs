using Payment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using Payment.ValueObjects;
using global::Payment.Enums;

namespace Payment.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionModel>
{
    public void Configure(EntityTypeBuilder<SubscriptionModel> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SubscriptionId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.Property(r => r.StartedAt);
        builder.Property(r => r.EndsAt);
        builder.Property(r => r.MaxRequestsPerDay);
        builder.Property(r => r.MaxTokensPerMonth);
        builder.Property(r => r.PlanName).HasMaxLength(200);

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Invoices)
               .WithOne()
               .HasForeignKey("SubscriptionId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Charges)
               .WithOne()
               .HasForeignKey("SubscriptionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Invoices).AutoInclude();
        builder.Navigation(s => s.Charges).AutoInclude();
    }
}
