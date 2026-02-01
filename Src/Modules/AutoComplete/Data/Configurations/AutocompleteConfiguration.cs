using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoComplete.Data.Configurations;

using AI.Common.Core;
using AutoComplete.ValueObjects;
using global::AutoComplete.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class AutocompleteConfiguration : IEntityTypeConfiguration<AutocompleteModel>
{
    public void Configure(EntityTypeBuilder<AutocompleteModel> builder)
    {

        builder.ToTable(nameof(AutocompleteModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}