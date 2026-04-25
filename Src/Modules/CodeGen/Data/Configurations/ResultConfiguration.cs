using CodeGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CodeGen.ValueObjects;
using AiOrchestration.ValueObjects;

namespace CodeGen.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<CodeGenerationResult>
{
    public void Configure(EntityTypeBuilder<CodeGenerationResult> builder)
    {
        builder.ToTable("codegen_results");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => CodeGenResultId.Of(dbId));

        builder.Property(r => r.Prompt)
            .HasConversion(p => p.Value, v => CodeGenerationPrompt.Of(v));

        builder.Property(r => r.Code)
            .HasConversion(p => p.Value, v => GeneratedCode.Of(v));

        builder.Property(r => r.Language)
            .HasConversion(p => p.Value, v => ProgrammingLanguage.Of(v));

        builder.Property(r => r.QualityLevel)
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
