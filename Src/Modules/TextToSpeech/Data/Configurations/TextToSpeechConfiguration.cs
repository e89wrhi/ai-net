using TextToSpeech.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TextToSpeech.Data.Configurations;

using AI.Common.Core;
using global::TextToSpeech.Enums;
using global::TextToSpeech.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class TextToSpeechConfiguration : IEntityTypeConfiguration<TextToSpeechSession>
{
    public void Configure(EntityTypeBuilder<TextToSpeechSession> builder)
    {

        builder.ToTable("text_to_speeches");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => TextToSpeechId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}