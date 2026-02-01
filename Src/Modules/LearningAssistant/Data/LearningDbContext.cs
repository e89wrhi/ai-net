using LearningAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LearningAssistant.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class LearningDbContext : AppDbContextBase
{
    public LearningDbContext(DbContextOptions<LearningDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<LearningDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<LearningSession> Sessions => Set<LearningSession>();
    public DbSet<LearningActivity> Activities => Set<LearningActivity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}