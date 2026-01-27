using Resume.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Resume.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ResumeDbContext : AppDbContextBase
{
    public ResumeDbContext(DbContextOptions<ResumeDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ResumeDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ResumeModel> Resumes => Set<ResumeModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}