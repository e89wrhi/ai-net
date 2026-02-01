using Meeting.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Meeting.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class MeetingDbContext : AppDbContextBase
{
    public MeetingDbContext(DbContextOptions<MeetingDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<MeetingDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<MeetingAnalysisSession> Sessions => Set<MeetingAnalysisSession>();
    public DbSet<MeetingTranscript> Transcripts => Set<MeetingTranscript>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}