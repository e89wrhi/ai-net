using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace CodeDebug.Data.Seed;

public class CodeDebugDataSeeder(
    CodeDebugDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedCodeDebugAsync();
        }
    }

    private async Task SeedCodeDebugAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.CodeDebugs);
            await dbContext.SaveChangesAsync();
        }
    }
}