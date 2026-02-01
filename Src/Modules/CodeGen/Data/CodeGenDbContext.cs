using CodeGen.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CodeGen.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class CodeGenDbContext : AppDbContextBase
{
    public CodeGenDbContext(DbContextOptions<CodeGenDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<CodeGenDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<CodeGenerationSession> Sessions => Set<CodeGenerationSession>();
    public DbSet<CodeGenerationResult> Results => Set<CodeGenerationResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}