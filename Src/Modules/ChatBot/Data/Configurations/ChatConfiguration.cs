using ChatBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using ChatBot.ValueObjects;
using global::ChatBot.Enums;
using AiOrchestration.ValueObjects;

namespace ChatBot.Data.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("chats");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => SessionId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.AiModelId)
            .HasConversion(id => id.Value, value => ModelId.Of(value));

        builder.Property(r => r.SessionStatus)
            .HasConversion<int>();

        builder.Property(r => r.Title).HasMaxLength(500).IsRequired();
        builder.Property(r => r.Summary).IsRequired();

        builder.ComplexProperty(r => r.Configuration, b => 
        {
             b.Property(c => c.Temperature)
                .HasConversion(v => v.Value, v => Temperature.Of(v))
                .HasColumnName("Temperature");
             b.Property(c => c.MaxTokens)
                .HasConversion(v => v.Value, v => TokenCount.Of(v))
                .HasColumnName("MaxTokens");
             b.Property(c => c.SystemPrompt)
                .HasConversion(v => v.Value, v => SystemPrompt.Of(v))
                .HasColumnName("SystemPrompt");
        });

        builder.ComplexProperty(r => r.TotalTokens, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalTokens");
        });

        builder.ComplexProperty(r => r.TotalCost, b => 
        {
             b.Property(t => t.Value).HasColumnName("TotalCost");
        });

        builder.Property(r => r.LastSentAt);
        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Messages)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Messages).AutoInclude();
    }
}