using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ImageCaption.Data.Seed;

public class ImageDataSeeder(
    ImageCaptionDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageAsync();
        }
    }

    private async Task SeedImageAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Images);
            await dbContext.SaveChangesAsync();
        }
    }
}