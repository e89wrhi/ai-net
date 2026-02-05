using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using AutoComplete.ValueObjects;
using global::AutoComplete.Enums;
using AiOrchestration.ValueObjects;

namespace AutoComplete.Data.Configurations;

public class AutocompleteConfiguration : IEntityTypeConfiguration<AutoCompleteSession>
{
    public void Configure(EntityTypeBuilder<AutoCompleteSession> builder)
    {
        builder.ToTable("autocompletes");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => AutoCompleteId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Temperature)
                .HasConversion(v => v.Value, v => Temperature.Of(v))
                .HasColumnName("Temperature");
             b.Property(c => c.MaxTokens)
                .HasConversion(v => v.Value, v => TokenCount.Of(v))
                .HasColumnName("MaxTokens");
             b.Property(c => c.Mode)
                .HasColumnName("Mode");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastRequestedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Requests)
               .WithOne()
               .HasForeignKey("AutoCompleteId")
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(s => s.Requests).AutoInclude();
    }
}