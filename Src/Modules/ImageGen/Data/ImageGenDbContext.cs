using ImageGen.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ImageGen.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ImageGenDbContext : AppDbContextBase
{
    public ImageGenDbContext(DbContextOptions<ImageGenDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ImageGenDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ImageGenerationSession> Sessions => Set<ImageGenerationSession>();
    public DbSet<ImageGenerationResult> Results => Set<ImageGenerationResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}