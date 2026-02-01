using ImageGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageGen.Data.Configurations;

using AI.Common.Core;
using ImageGen.ValueObjects;
using global::ImageGen.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ImageGenConfiguration : IEntityTypeConfiguration<ImageGenModel>
{
    public void Configure(EntityTypeBuilder<ImageGenModel> builder)
    {

        builder.ToTable(nameof(ImageGenModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}