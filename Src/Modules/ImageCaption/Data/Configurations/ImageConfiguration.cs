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

public class ImageConfiguration : IEntityTypeConfiguration<ImageCaptionSession>
{
    public void Configure(EntityTypeBuilder<ImageCaptionSession> builder)
    {

        builder.ToTable("image_captions");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ImageCaptionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}