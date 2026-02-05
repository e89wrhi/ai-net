using SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using SpeechToText.ValueObjects;
using global::SpeechToText.Enums;
using AiOrchestration.ValueObjects;

namespace SpeechToText.Data.Configurations;

public class SpeechToTextConfiguration : IEntityTypeConfiguration<SpeechToTextSession>
{
    public void Configure(EntityTypeBuilder<SpeechToTextSession> builder)
    {
        builder.ToTable("speech_to_text_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SpeechToTextId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Language)
                .HasConversion(v => v.Value, v => LanguageCode.Of(v))
                .HasColumnName("Language");
             b.Property(c => c.IncludePunctuation)
                .HasColumnName("IncludePunctuation");
             b.Property(c => c.DetailLevel)
                .HasConversion<int>()
                .HasColumnName("DetailLevel");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastTranscribedAt);
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