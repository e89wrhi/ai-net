using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Sentiment.Data.Seed;

public class SentimentDataSeeder(
    SentimentDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSentimentAsync();
        }
    }

    private async Task SeedSentimentAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Sentiments);
            await dbContext.SaveChangesAsync();
        }
    }
}