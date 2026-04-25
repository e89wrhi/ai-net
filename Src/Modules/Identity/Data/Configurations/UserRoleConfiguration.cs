namespace Identity.Data.Configurations;

using global::Identity.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}