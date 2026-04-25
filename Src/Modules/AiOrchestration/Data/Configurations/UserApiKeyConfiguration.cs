using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiOrchestration.Data.Configurations;

public class UserApiKeyConfiguration : IEntityTypeConfiguration<UserApiKey>
{
    public void Configure(EntityTypeBuilder<UserApiKey> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(v => v.Value, v => ApiKeyId.Of(v));

        builder.Property(x => x.ProviderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ApiKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(200);
    }
}
