using Summary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Summary.Data.Configurations;

using AI.Common.Core;
using Summary.ValueObjects;
using global::Summary.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class SummaryConfiguration : IEntityTypeConfiguration<SummaryModel>
{
    public void Configure(EntityTypeBuilder<SummaryModel> builder)
    {

        builder.ToTable(nameof(SummaryModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}