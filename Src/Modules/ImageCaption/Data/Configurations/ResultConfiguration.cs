using ImageCaption.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageCaption.ValueObjects;
using AiOrchestration.ValueObjects;

namespace ImageCaption.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<ImageCaptionResult>
{
    public void Configure(EntityTypeBuilder<ImageCaptionResult> builder)
    {
        builder.ToTable("image_caption_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ImageCaptionResultId.Of(dbId));

        builder.Property(r => r.Image)
            .HasConversion(p => p.Value, v => ImageSource.Of(v));

        builder.Property(r => r.Caption)
            .HasConversion(p => p.Value, v => CaptionText.Of(v));

        builder.ComplexProperty(r => r.Confidence, b => 
        {
             b.Property(t => t.Value).HasColumnName("Confidence");
        });

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.CaptionedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
