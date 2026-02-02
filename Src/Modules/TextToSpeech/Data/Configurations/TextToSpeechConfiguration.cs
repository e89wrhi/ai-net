using TextToSpeech.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TextToSpeech.Data.Configurations;

using AI.Common.Core;
using TextToSpeech.ValueObjects;
using global::TextToSpeech.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class TextToSpeechConfiguration : IEntityTypeConfiguration<TextToSpeechModel>
{
    public void Configure(EntityTypeBuilder<TextToSpeechModel> builder)
    {

        builder.ToTable("text_to_speeches");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}