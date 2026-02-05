using ImageEdit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using ImageEdit.ValueObjects;
using global::ImageEdit.Enums;
using AiOrchestration.ValueObjects;

namespace ImageEdit.Data.Configurations;

public class ImageEditConfiguration : IEntityTypeConfiguration<ImageEditSession>
{
    public void Configure(EntityTypeBuilder<ImageEditSession> builder)
    {
        builder.ToTable("image_edit_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ImageEditId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Quality)
                .HasConversion<int>()
                .HasColumnName("Quality");
             b.Property(c => c.OutputFormat)
                .HasConversion<int>()
                .HasColumnName("OutputFormat");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastEditedAt);
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