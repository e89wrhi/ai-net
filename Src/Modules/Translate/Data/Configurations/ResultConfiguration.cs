using Translate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Translate.ValueObjects;
using AiOrchestration.ValueObjects;

namespace Translate.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<TranslationResult>
{
    public void Configure(EntityTypeBuilder<TranslationResult> builder)
    {
        builder.ToTable("translation_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => TranslateResultId.Of(dbId));

        builder.Property(r => r.OriginalText);

        builder.Property(r => r.TranslatedText)
            .HasConversion(p => p.Value, v => TranslatedText.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.TranslatedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
