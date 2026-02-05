using ImageEdit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageEdit.ValueObjects;
using AiOrchestration.ValueObjects;

namespace ImageEdit.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<ImageEditResult>
{
    public void Configure(EntityTypeBuilder<ImageEditResult> builder)
    {
        builder.ToTable("image_edit_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ImageEditResultId.Of(dbId));

        builder.Property(r => r.OriginalImage)
            .HasConversion(p => p.Value, v => ImageSource.Of(v));

        builder.Property(r => r.ResultImage)
            .HasConversion(p => p.Value, v => EditedImage.Of(v));

        builder.Property(r => r.Prompt)
            .HasConversion(p => p.Value, v => ImageEditPrompt.Of(v));

        builder.Property(r => r.Operation)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.EditedAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
