using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoComplete.ValueObjects;
using AiOrchestration.ValueObjects;

namespace AutoComplete.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<AutoCompleteRequest>
{
    public void Configure(EntityTypeBuilder<AutoCompleteRequest> builder)
    {
        builder.ToTable("autocomplete_requests");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => AutoCompleteRequestId.Of(dbId));

        builder.Property(r => r.Prompt)
            .HasConversion(p => p.Value, v => AutoCompletePrompt.Of(v));

        builder.Property(r => r.Suggestion)
            .HasConversion(s => s.Value, v => AutoCompleteSuggestion.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.RequestedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
