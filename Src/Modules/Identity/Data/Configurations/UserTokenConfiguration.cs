namespace Identity.Data.Configurations;

using global::Identity.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("user_tokens");

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}