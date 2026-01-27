using LearningAssistant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assistant.Data.Configurations;

using AI.Common.Core;
using LearningAssistant.ValueObjects;
using global::LearningAssistant.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;

public class ProfileConfiguration : IEntityTypeConfiguration<ProfileModel>
{
    public void Configure(EntityTypeBuilder<ProfileModel> builder)
    {

        builder.ToTable(nameof(ProfileModel));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(itemId => itemId.Value, dbId => ProfileId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

    }
}