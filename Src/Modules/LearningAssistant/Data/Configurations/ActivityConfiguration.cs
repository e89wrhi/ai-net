using LearningAssistant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearningAssistant.ValueObjects;
using AiOrchestration.ValueObjects;

namespace LearningAssistant.Data.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<LearningActivity>
{
    public void Configure(EntityTypeBuilder<LearningActivity> builder)
    {
        builder.ToTable("learning_activities");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => ActivityId.Of(dbId));

        builder.Property(r => r.Topic)
            .HasConversion(p => p.Value, v => LearningTopic.Of(v));

        builder.Property(r => r.Content)
            .HasConversion(p => p.Value, v => LearningContent.Of(v));

        builder.Property(r => r.Outcome)
            .HasConversion(p => p.Value, v => LearningOutcome.Of(v));

        builder.ComplexProperty(r => r.TokenUsed, b => 
        {
             b.Property(t => t.Value).HasColumnName("TokenUsed");
        });

        builder.ComplexProperty(r => r.Cost, b => 
        {
             b.Property(t => t.Value).HasColumnName("Cost");
        });

        builder.Property(r => r.ActivityAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
