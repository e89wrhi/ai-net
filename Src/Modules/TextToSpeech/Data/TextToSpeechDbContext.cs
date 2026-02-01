using TextToSpeech.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace TextToSpeech.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class TextToSpeechDbContext : AppDbContextBase
{
    public TextToSpeechDbContext(DbContextOptions<TextToSpeechDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<TextToSpeechDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<TextToSpeechSession> Sessions => Set<TextToSpeechSession>();
    public DbSet<TextToSpeechResult> Results => Set<TextToSpeechResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}