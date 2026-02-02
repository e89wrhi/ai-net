using CodeDebug.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeDebug.Data.Configurations;

using AI.Common.Core;
using CodeDebug.ValueObjects;
using global::CodeDebug.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class CodeDebugConfiguration : IEntityTypeConfiguration<CodeDebugSession>
{
    public void Configure(EntityTypeBuilder<CodeDebugSession> builder)
    {

        builder.ToTable("code_debugs");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}