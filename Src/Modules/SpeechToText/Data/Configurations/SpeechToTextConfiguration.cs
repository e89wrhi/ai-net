using SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpeechToText.Data.Configurations;

using AI.Common.Core;
using global::SpeechToText.ValueObjects;
using global::SpeechToText.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class SpeechToTextConfiguration : IEntityTypeConfiguration<SpeechToTextSession>
{
    public void Configure(EntityTypeBuilder<SpeechToTextSession> builder)
    {

        builder.ToTable("speech_to_texts");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SpeechToTextId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}