using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using AutoComplete.ValueObjects;
using global::AutoComplete.Enums;

namespace AutoComplete.Data.Configurations;

public class AutocompleteConfiguration : IEntityTypeConfiguration<AutoCompleteSession>
{
    public void Configure(EntityTypeBuilder<AutoCompleteSession> builder)
    {

        builder.ToTable("autocompletes");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => AutoCompleteId.Of(dbId));
        
        // Value Objects conversions if needed
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Configuration)
             .HasConversion(c => c.Value, value => AutoCompleteConfiguration.Of(value));

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Requests)
               .WithOne()
               .OnDelete(DeleteBehavior.Cascade);
    }
}