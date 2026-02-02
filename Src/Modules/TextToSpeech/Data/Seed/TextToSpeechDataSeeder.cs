using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace TextToSpeech.Data.Seed;

public class TextToSpeechDataSeeder(
    TextToSpeechDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedTextToSpeechAsync();
        }
    }

    private async Task SeedTextToSpeechAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.TextToSpeechs);
            await dbContext.SaveChangesAsync();
        }
    }
}