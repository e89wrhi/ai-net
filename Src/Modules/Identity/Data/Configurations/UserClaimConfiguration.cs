namespace Identity.Data.Configurations;

using global::Identity.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("user_claims");

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}