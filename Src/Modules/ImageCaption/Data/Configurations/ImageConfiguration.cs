using ImageCaption.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Image.Data.Configurations;

using AI.Common.Core;
using ImageCaption.ValueObjects;
using global::ImageCaption.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ImageConfiguration : IEntityTypeConfiguration<ImageModel>
{
    public void Configure(EntityTypeBuilder<ImageModel> builder)
    {

        builder.ToTable(nameof(ImageModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ImageCaptionResultId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}