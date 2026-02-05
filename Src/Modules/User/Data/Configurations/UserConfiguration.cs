using User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AI.Common.Core;
using User.ValueObjects;
using global::User.Enums;

namespace User.Data.Configurations;

public class UserActivitySessionConfiguration : IEntityTypeConfiguration<UserActivitySession>
{
    public void Configure(EntityTypeBuilder<UserActivitySession> builder)
    {
        builder.ToTable("user_activity_sessions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => UserActivityId.Of(dbId));
        
        builder.Property(r => r.UserId)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.Property(r => r.LastActivityAt);
        builder.Property(r => r.TotalActions);

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
        
        builder.HasMany(s => s.Actions)
               .WithOne()
               .HasForeignKey("SessionId")
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.Navigation(s => s.Actions).AutoInclude();
    }
}