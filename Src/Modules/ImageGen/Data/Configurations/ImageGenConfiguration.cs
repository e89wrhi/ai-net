using ImageGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageGen.Data.Configurations;

using AI.Common.Core;
using global::ImageGen.ValueObjects;
using global::ImageGen.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ImageGenConfiguration : IEntityTypeConfiguration<ImageGenerationSession>
{
    public void Configure(EntityTypeBuilder<ImageGenerationSession> builder)
    {

        builder.ToTable("image_gens");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ImageGenId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}