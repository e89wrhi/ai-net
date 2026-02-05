using CodeDebug.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using CodeDebug.ValueObjects;
using global::CodeDebug.Enums;
using AiOrchestration.ValueObjects;

namespace CodeDebug.Data.Configurations;

public class CodeDebugConfiguration : IEntityTypeConfiguration<CodeDebugSession>
{
    public void Configure(EntityTypeBuilder<CodeDebugSession> builder)
    {
        builder.ToTable("code_debug_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => CodeDebugId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Depth)
                .HasConversion<int>()
                .HasColumnName("Depth");
             b.Property(c => c.IncludeSuggestions)
                .HasColumnName("IncludeSuggestions");
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
        
        builder.HasMany(s => s.Reports)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Reports).AutoInclude();
    }
}