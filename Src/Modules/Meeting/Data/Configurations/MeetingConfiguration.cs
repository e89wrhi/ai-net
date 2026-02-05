using Meeting.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meeting.Data.Configurations;

using AI.Common.Core;
using Meeting.ValueObjects;
using global::Meeting.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class MeetingConfiguration : IEntityTypeConfiguration<MeetingAnalysisSession>
{
    public void Configure(EntityTypeBuilder<MeetingAnalysisSession> builder)
    {

        builder.ToTable("meetings");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => MeetingId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}