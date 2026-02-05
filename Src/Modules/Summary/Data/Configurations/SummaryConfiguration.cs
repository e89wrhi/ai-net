using Summary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Summary.Data.Configurations;

using AI.Common.Core;
using global::Summary.ValueObjects;
using global::Summary.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class SummaryConfiguration : IEntityTypeConfiguration<TextSummarySession>
{
    public void Configure(EntityTypeBuilder<TextSummarySession> builder)
    {

        builder.ToTable("summaries");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SummaryId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}