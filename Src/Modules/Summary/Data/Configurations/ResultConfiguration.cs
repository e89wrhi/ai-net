using Summary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Summary.ValueObjects;
using AiOrchestration.ValueObjects;

namespace Summary.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<TextSummaryResult>
{
    public void Configure(EntityTypeBuilder<TextSummaryResult> builder)
    {
        builder.ToTable("summary_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SummaryResultId.Of(dbId));

        builder.Property(r => r.OriginalText);

        builder.Property(r => r.Summary)
            .HasConversion(p => p.Value, v => SummaryText.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.SummarizedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
