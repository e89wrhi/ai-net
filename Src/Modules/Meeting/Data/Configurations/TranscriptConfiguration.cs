using Meeting.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meeting.ValueObjects;
using AiOrchestration.ValueObjects;

namespace Meeting.Data.Configurations;

public class TranscriptConfiguration : IEntityTypeConfiguration<MeetingTranscript>
{
    public void Configure(EntityTypeBuilder<MeetingTranscript> builder)
    {
        builder.ToTable("meeting_transcripts");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => TranscriptId.Of(dbId));

        builder.Property(r => r.RawTranscript);

        builder.Property(r => r.Summary)
            .HasConversion(p => p.Value, v => TranscriptSummary.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.RecordedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
