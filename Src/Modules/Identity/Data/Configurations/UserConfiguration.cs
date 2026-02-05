namespace Identity.Data.Configurations;

using global::Identity.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(r => r.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.LastName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.PassPortNumber).HasMaxLength(50);

        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}