using SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeechToText.ValueObjects;
using AiOrchestration.ValueObjects;

namespace SpeechToText.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<SpeechToTextResult>
{
    public void Configure(EntityTypeBuilder<SpeechToTextResult> builder)
    {
        builder.ToTable("speech_to_text_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SpeechToTextResultId.Of(dbId));

        builder.Property(r => r.Audio)
            .HasConversion(p => p.Value, v => AudioSource.Of(v));

        builder.Property(r => r.Transcript)
            .HasConversion(p => p.Value, v => Transcript.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.TranscribedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
