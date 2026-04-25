using AiOrchestration.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

namespace AiOrchestration.Data;

public sealed class AiOrchestrationDbContext : AppDbContextBase
{
    public AiOrchestrationDbContext(DbContextOptions<AiOrchestrationDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<AiOrchestrationDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<UserApiKey> UserApiKeys => Set<UserApiKey>();
    public DbSet<AiUsage> UsageLogs => Set<AiUsage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
