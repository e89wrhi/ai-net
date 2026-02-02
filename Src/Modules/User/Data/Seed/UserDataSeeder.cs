using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace User.Data.Seed;

public class UserDataSeeder(
    UserDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedUserAsync();
        }
    }

    private async Task SeedUserAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Users);
            await dbContext.SaveChangesAsync();
        }
    }
}