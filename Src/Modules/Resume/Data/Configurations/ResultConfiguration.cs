using Resume.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resume.ValueObjects;
using AiOrchestration.ValueObjects;

namespace Resume.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<ResumeAnalysisResult>
{
    public void Configure(EntityTypeBuilder<ResumeAnalysisResult> builder)
    {
        builder.ToTable("resume_analysis_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ResultId.Of(dbId));

        builder.Property(r => r.Resume)
            .HasConversion(p => p.Value, v => ResumeFile.Of(v));

        builder.Property(r => r.Summary)
            .HasConversion(p => p.Value, v => ResumeSummary.Of(v));

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
