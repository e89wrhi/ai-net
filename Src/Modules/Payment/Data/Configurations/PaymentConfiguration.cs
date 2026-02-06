using Payment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using Payment.ValueObjects;
using global::Payment.Enums;

namespace Payment.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<PaymentSession>
{
    public void Configure(EntityTypeBuilder<PaymentSession> builder)
    {
        builder.ToTable("payment_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => PaymentId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.PaymentMethod, b =>
        {
            b.Property(m => m.AccountNumber).HasColumnName("AccountNumber");
            b.Property(m => m.Provider).HasColumnName("Provider");
        });

        builder.ComplexProperty(r => r.Amount, b => 
        {
             b.Property(m => m.Amount).HasColumnName("Amount");
             b.Property(m => m.Currency).HasColumnName("Currency");
        });

        builder.Property(r => r.Currency)
            .HasConversion(c => c.Value, v => CurrencyCode.Of(v));

        builder.Property(r => r.LastUpdatedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Invoices)
               .WithOne()
               .HasForeignKey("PaymentSessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Invoices).AutoInclude();
    }
}