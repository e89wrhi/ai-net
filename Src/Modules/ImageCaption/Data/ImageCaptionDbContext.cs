using ImageCaption.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ImageCaption.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ImageCaptionDbContext : AppDbContextBase
{
    public ImageCaptionDbContext(DbContextOptions<ImageCaptionDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ImageCaptionDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ImageCaptionSession> Sessions => Set<ImageCaptionSession>();
    public DbSet<ImageCaptionResult> Results => Set<ImageCaptionResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}