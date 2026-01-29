using LearningAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LearningAssistant.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class AssistantDbContext : AppDbContextBase
{
    public AssistantDbContext(DbContextOptions<AssistantDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<AssistantDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ProfileModel> Profiles => Set<ProfileModel>();
    public DbSet<QuizModel> Quizzes => Set<QuizModel>();
    public DbSet<LessonModel> Lessons => Set<LessonModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}