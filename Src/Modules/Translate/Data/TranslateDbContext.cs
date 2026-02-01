using Translate.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Translate.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class TranslateDbContext : AppDbContextBase
{
    public TranslateDbContext(DbContextOptions<TranslateDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<TranslateDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<TranslationSession> Sessions => Set<TranslationSession>();
    public DbSet<TranslationResult> Results => Set<TranslationResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}