using Summary.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Summary.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class SummaryDbContext : AppDbContextBase
{
    public SummaryDbContext(DbContextOptions<SummaryDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<SummaryDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<TextSummarySession> Sessions => Set<TextSummarySession>();
    public DbSet<TextSummaryResult> Results => Set<TextSummaryResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}