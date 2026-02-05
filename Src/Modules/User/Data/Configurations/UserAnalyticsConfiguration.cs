using User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.ValueObjects;

namespace User.Data.Configurations;

public class UserAnalyticsConfiguration : IEntityTypeConfiguration<UserAnalytics>
{
    public void Configure(EntityTypeBuilder<UserAnalytics> builder)
    {
        builder.ToTable("user_analytics");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion(itemId => itemId.Value, dbId => UserAnalyticsId.Of(dbId));

        builder.Property(r => r.User)
            .HasConversion(id => id.Value, value => UserId.Of(value));

        builder.Property(r => r.TotalRequests);
        builder.Property(r => r.TodayRequests);
        builder.Property(r => r.WeekRequests);
        builder.Property(r => r.MonthRequests);
        builder.Property(r => r.LastUpdatedAt);

        builder.Property(r => r.CreatedAt);
        builder.Property(r => r.CreatedBy);
        builder.Property(r => r.LastModified);
        builder.Property(r => r.LastModifiedBy);
        builder.Property(r => r.IsDeleted);
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
