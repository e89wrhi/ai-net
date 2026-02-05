using Payment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.ValueObjects;
using global::Payment.Enums;

namespace Payment.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => InvoiceId.Of(dbId));

        builder.Property(r => r.InvoiceNumber).HasMaxLength(100);

        builder.ComplexProperty(r => r.Amount, b => 
        {
             b.Property(m => m.Amount).HasColumnName("Amount");
             b.Property(m => m.Currency).HasColumnName("Currency");
        });

        builder.Property(r => r.Currency)
            .HasConversion(c => c.Value, v => CurrencyCode.Of(v));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.Property(r => r.IssuedAt);
        builder.Property(r => r.PaidAt);
        
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
