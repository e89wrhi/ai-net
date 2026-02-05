using CodeDebug.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeDebug.ValueObjects;
using AiOrchestration.ValueObjects;

namespace CodeDebug.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<CodeDebugReport>
{
    public void Configure(EntityTypeBuilder<CodeDebugReport> builder)
    {
        builder.ToTable("code_debug_reports");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => CodeDebugReportId.Of(dbId));

        builder.Property(r => r.Code)
            .HasConversion(p => p.Value, v => SourceCode.Of(v));

        builder.Property(r => r.Language)
            .HasConversion<int>();

        builder.Property(r => r.Summary)
            .HasConversion(p => p.Value, v => DebugSummary.Of(v));

        builder.ComplexProperty(r => r.IssueCount, b => 
        {
             b.Property(t => t.Value).HasColumnName("IssueCount");
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
