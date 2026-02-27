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

        // Configure Value Objects conversions if needed, but AppDbContextBase usually handles basic ones.
        // For custom records like ModelId, we might need configuration.
        
        builder.Entity<UserApiKey>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasConversion(v => v.Value, v => ApiKeyId.Of(v));
        });

        builder.Entity<AiUsage>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasConversion(v => v.Value, v => AiUsageId.Of(v));
            e.Property(x => x.ModelId).HasConversion(v => v.Value, v => AiOrchestration.ValueObjects.ModelId.Of(v));
        });
    }
}
