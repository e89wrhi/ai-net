using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ImageGen.Data.Seed;

public class ImageGenDataSeeder(
    ImageGenDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageGenAsync();
        }
    }

    private async Task SeedImageGenAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.ImageGens);
            await dbContext.SaveChangesAsync();
        }
    }
}