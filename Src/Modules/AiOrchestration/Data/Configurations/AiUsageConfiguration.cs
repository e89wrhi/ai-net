using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiOrchestration.Data.Configurations;

public class AiUsageConfiguration : IEntityTypeConfiguration<AiUsage>
{
    public void Configure(EntityTypeBuilder<AiUsage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(v => v.Value, v => AiUsageId.Of(v));

        builder.Property(x => x.ModelId)
            .HasConversion(v => v.Value, v => AiOrchestration.ValueObjects.ModelId.Of(v))
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ProviderName)
            .HasMaxLength(100);
    }
}
