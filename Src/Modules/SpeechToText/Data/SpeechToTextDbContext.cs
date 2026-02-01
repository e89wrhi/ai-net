using SpeechToText.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SpeechToText.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class SpeechToTextDbContext : AppDbContextBase
{
    public SpeechToTextDbContext(DbContextOptions<SpeechToTextDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<SpeechToTextDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<SpeechToTextSession> Sessions => Set<SpeechToTextSession>();
    public DbSet<SpeechToTextResult> Results => Set<SpeechToTextResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}