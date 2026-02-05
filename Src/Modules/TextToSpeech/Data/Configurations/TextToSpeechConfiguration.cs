using TextToSpeech.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using TextToSpeech.ValueObjects;
using global::TextToSpeech.Enums;
using AiOrchestration.ValueObjects;

namespace TextToSpeech.Data.Configurations;

public class TextToSpeechConfiguration : IEntityTypeConfiguration<TextToSpeechSession>
{
    public void Configure(EntityTypeBuilder<TextToSpeechSession> builder)
    {
        builder.ToTable("text_to_speech_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => TextToSpeechId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Voice)
                .HasConversion<int>()
                .HasColumnName("Voice");
             b.Property(c => c.Speed)
                .HasConversion(v => v.Value, v => SpeechSpeed.Of(v))
                .HasColumnName("Speed");
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

        builder.Property(r => r.LastSynthesizedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Results)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Results).AutoInclude();
    }
}