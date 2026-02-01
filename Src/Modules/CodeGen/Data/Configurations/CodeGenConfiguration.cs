using CodeGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeGen.Data.Configurations;

using AI.Common.Core;
using CodeGen.ValueObjects;
using global::CodeGen.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class CodeGenConfiguration : IEntityTypeConfiguration<CodeGenModel>
{
    public void Configure(EntityTypeBuilder<CodeGenModel> builder)
    {

        builder.ToTable(nameof(CodeGenModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}