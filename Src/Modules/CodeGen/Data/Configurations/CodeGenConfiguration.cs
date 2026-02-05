using CodeGen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeGen.Data.Configurations;

using AI.Common.Core;
using global::CodeGen.ValueObjects;
using global::CodeGen.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class CodeGenConfiguration : IEntityTypeConfiguration<CodeGenerationSession>
{
    public void Configure(EntityTypeBuilder<CodeGenerationSession> builder)
    {

        builder.ToTable("code_gens");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => CodeGenId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}