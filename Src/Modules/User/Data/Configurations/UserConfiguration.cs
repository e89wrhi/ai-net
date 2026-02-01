using User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.Data.Configurations;

using AI.Common.Core;
using User.ValueObjects;
using global::User.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class UserConfiguration : IEntityTypeConfiguration<UserActivitySession>
{
    public void Configure(EntityTypeBuilder<UserActivitySession> builder)
    {

        builder.ToTable(nameof(UserActivitySession));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => UserId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}