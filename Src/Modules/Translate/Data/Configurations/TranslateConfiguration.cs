using Translate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Translate.Data.Configurations;

using AI.Common.Core;
using Translate.ValueObjects;
using global::Translate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class TranslateConfiguration : IEntityTypeConfiguration<TranslateModel>
{
    public void Configure(EntityTypeBuilder<TranslateModel> builder)
    {

        builder.ToTable(nameof(TranslateModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}