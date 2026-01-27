using ChatBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Data.Configurations;

using AI.Common.Core;
using ChatBot.ValueObjects;
using global::ChatBot.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ChatConfiguration : IEntityTypeConfiguration<ChatModel>
{
    public void Configure(EntityTypeBuilder<ChatModel> builder)
    {

        builder.ToTable(nameof(ChatModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => SessionId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}