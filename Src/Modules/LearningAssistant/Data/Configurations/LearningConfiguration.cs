using LearningAssistant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using LearningAssistant.ValueObjects;
using global::LearningAssistant.Enums;
using AiOrchestration.ValueObjects;

namespace LearningAssistant.Data.Configurations;

public class LearningConfiguration : IEntityTypeConfiguration<LearningSession>
{
    public void Configure(EntityTypeBuilder<LearningSession> builder)
    {
        builder.ToTable("learning_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => LearningId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Mode)
                .HasConversion<int>()
                .HasColumnName("Mode");
             b.Property(c => c.Difficulty)
                .HasConversion<int>()
                .HasColumnName("Difficulty");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastInteractionAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Activities)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Activities).AutoInclude();
    }
}