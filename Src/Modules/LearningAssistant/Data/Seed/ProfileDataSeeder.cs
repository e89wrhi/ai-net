using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace LearningAssistant.Data.Seed;

public class ProfileDataSeeder(
    LearningDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedAssistantAsync();
        }
    }

    private async Task SeedAssistantAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Profiles);
            await dbContext.SaveChangesAsync();
        }
    }
}