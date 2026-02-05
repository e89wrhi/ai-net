using Meeting.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using Meeting.ValueObjects;
using global::Meeting.Enums;
using AiOrchestration.ValueObjects;

namespace Meeting.Data.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<MeetingAnalysisSession>
{
    public void Configure(EntityTypeBuilder<MeetingAnalysisSession> builder)
    {
        builder.ToTable("meeting_analysis_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => MeetingId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.IncludeActionItems)
                .HasColumnName("IncludeActionItems");
             b.Property(c => c.IncludeDecisions)
                .HasColumnName("IncludeDecisions");
             b.Property(c => c.Language)
                .HasConversion(v => v.Value, v => LanguageCode.Of(v))
                .HasColumnName("Language");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastAnalyzedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Transcripts)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Transcripts).AutoInclude();
    }
}