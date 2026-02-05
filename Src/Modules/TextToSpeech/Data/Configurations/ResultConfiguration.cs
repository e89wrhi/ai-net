using TextToSpeech.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TextToSpeech.ValueObjects;
using AiOrchestration.ValueObjects;

namespace TextToSpeech.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<TextToSpeechResult>
{
    public void Configure(EntityTypeBuilder<TextToSpeechResult> builder)
    {
        builder.ToTable("text_to_speech_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => TextToSpeechResultId.Of(dbId));

        builder.Property(r => r.OriginalText);

        builder.Property(r => r.Speech)
            .HasConversion(p => p.Value, v => SynthesizedSpeech.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.SynthesizedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
