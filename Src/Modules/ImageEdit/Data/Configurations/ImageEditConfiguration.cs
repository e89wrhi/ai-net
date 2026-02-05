using ImageEdit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageEdit.Data.Configurations;

using AI.Common.Core;
using global::ImageEdit.ValueObjects;
using global::ImageEdit.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ImageEditConfiguration : IEntityTypeConfiguration<ImageEditSession>
{
    public void Configure(EntityTypeBuilder<ImageEditSession> builder)
    {

        builder.ToTable("image_edits");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ImageEditId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}