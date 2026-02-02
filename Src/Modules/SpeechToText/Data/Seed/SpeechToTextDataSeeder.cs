using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace SpeechToText.Data.Seed;

public class SpeechToTextDataSeeder(
    SpeechToTextDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSpeechToTextAsync();
        }
    }

    private async Task SeedSpeechToTextAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.SpeechToTexts);
            await dbContext.SaveChangesAsync();
        }
    }
}