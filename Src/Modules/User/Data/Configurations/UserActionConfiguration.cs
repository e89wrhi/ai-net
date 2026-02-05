using User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.ValueObjects;

namespace User.Data.Configurations;

public class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
{
    public void Configure(EntityTypeBuilder<UserAction> builder)
    {
        builder.ToTable("user_actions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => UserActionId.Of(dbId));

        builder.Property(r => r.ActionType).HasMaxLength(100);
        builder.Property(r => r.Description);
        builder.Property(r => r.PerformedAt);

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
