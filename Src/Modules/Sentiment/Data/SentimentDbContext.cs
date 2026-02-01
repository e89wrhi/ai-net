using Sentiment.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Sentiment.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class SentimentDbContext : AppDbContextBase
{
    public SentimentDbContext(DbContextOptions<SentimentDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<SentimentDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<TextSentimentSession> Sessions => Set<TextSentimentSession>();
    public DbSet<TextSentimentResult> Results => Set<TextSentimentResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}