using CodeDebug.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CodeDebug.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class CodeDebugDbContext : AppDbContextBase
{
    public CodeDebugDbContext(DbContextOptions<CodeDebugDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<CodeDebugDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<CodeDebugSession> Sessions => Set<CodeDebugSession>();
    public DbSet<CodeDebugReport> Reports => Set<CodeDebugReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}