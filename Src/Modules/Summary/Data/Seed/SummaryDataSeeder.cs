using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Summary.Data.Seed;

public class SummaryDataSeeder(
    SummaryDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSummaryAsync();
        }
    }

    private async Task SeedSummaryAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Summarys);
            await dbContext.SaveChangesAsync();
        }
    }
}