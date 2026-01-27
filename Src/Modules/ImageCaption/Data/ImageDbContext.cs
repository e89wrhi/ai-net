using ImageCaption.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ImageCaption.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ImageDbContext : AppDbContextBase
{
    public ImageDbContext(DbContextOptions<ImageDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ImageDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ImageModel> Images => Set<ImageModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}