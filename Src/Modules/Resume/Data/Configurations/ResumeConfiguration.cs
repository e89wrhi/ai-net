using Resume.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Resume.Data.Configurations;

using AI.Common.Core;
using Resume.ValueObjects;
using global::Resume.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ResumeConfiguration : IEntityTypeConfiguration<ResumeModel>
{
    public void Configure(EntityTypeBuilder<ResumeModel> builder)
    {

        builder.ToTable("resumes");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ResumeId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}