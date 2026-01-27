using User.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace User.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class UserDbContext : AppDbContextBase
{
    public UserDbContext(DbContextOptions<UserDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<UserDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<UserModel> Users => Set<UserModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}