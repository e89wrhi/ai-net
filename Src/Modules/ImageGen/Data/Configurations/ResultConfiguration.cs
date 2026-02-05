using ImageGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageGen.ValueObjects;
using AiOrchestration.ValueObjects;

namespace ImageGen.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<ImageGenerationResult>
{
    public void Configure(EntityTypeBuilder<ImageGenerationResult> builder)
    {
        builder.ToTable("image_gen_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ImageGenResultId.Of(dbId));

        builder.Property(r => r.Prompt)
            .HasConversion(p => p.Value, v => ImageGenerationPrompt.Of(v));

        builder.Property(r => r.Image)
            .HasConversion(p => p.Value, v => GeneratedImage.Of(v));

        builder.Property(r => r.Size)
            .HasConversion<int>();

        builder.Property(r => r.Style)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.GeneratedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
