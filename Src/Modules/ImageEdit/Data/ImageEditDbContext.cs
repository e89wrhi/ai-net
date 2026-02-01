using ImageEdit.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ImageEdit.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ImageEditDbContext : AppDbContextBase
{
    public ImageEditDbContext(DbContextOptions<ImageEditDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ImageEditDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ImageEditSession> Sessions => Set<ImageEditSession>();
    public DbSet<ImageEditResult> Results => Set<ImageEditResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}