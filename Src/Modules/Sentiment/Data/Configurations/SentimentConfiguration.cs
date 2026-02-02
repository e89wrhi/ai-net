using Sentiment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sentiment.Data.Configurations;

using AI.Common.Core;
using Sentiment.ValueObjects;
using global::Sentiment.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class SentimentConfiguration : IEntityTypeConfiguration<SentimentModel>
{
    public void Configure(EntityTypeBuilder<SentimentModel> builder)
    {

        builder.ToTable("sentiments");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}