using ImageEdit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageEdit.Data.Configurations;

using AI.Common.Core;
using ImageEdit.ValueObjects;
using global::ImageEdit.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ImageEditConfiguration : IEntityTypeConfiguration<ImageEditModel>
{
    public void Configure(EntityTypeBuilder<ImageEditModel> builder)
    {

        builder.ToTable(nameof(ImageEditModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}