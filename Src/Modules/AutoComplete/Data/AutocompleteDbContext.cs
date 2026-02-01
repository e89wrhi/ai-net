using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoComplete.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class AutocompleteDbContext : AppDbContextBase
{
    public AutocompleteDbContext(DbContextOptions<AutocompleteDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<AutocompleteDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<AutoCompleteSession> Sessions => Set<AutoCompleteSession>();
    public DbSet<AutoCompleteRequest> Requests => Set<AutoCompleteRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}