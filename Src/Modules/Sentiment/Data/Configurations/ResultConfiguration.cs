using Sentiment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentiment.ValueObjects;
using AiOrchestration.ValueObjects;

namespace Sentiment.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<TextSentimentResult>
{
    public void Configure(EntityTypeBuilder<TextSentimentResult> builder)
    {
        builder.ToTable("sentiment_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SentimentResultId.Of(dbId));

        builder.Property(r => r.Text);

        builder.Property(r => r.Sentiment)
            .HasConversion(p => p.Value, v => SentimentText.Of(v));

        builder.ComplexProperty(r => r.Score, b => 
        {
             b.Property(t => t.Value).HasColumnName("Score");
        });

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.AnalyzedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
