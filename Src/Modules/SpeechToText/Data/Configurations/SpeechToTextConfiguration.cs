using SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpeechToText.Data.Configurations;

using AI.Common.Core;
using SpeechToText.ValueObjects;
using global::SpeechToText.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class SpeechToTextConfiguration : IEntityTypeConfiguration<SpeechToTextModel>
{
    public void Configure(EntityTypeBuilder<SpeechToTextModel> builder)
    {

        builder.ToTable("speech_to_texts");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}