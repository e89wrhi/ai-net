using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Translate.Data.Seed;

public class TranslateDataSeeder(
    TranslateDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedTranslateAsync();
        }
    }

    private async Task SeedTranslateAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Translates);
            await dbContext.SaveChangesAsync();
        }
    }
}