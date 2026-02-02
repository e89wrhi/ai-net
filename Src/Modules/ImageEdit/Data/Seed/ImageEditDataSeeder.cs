using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ImageEdit.Data.Seed;

public class ImageEditDataSeeder(
    ImageEditDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageEditAsync();
        }
    }

    private async Task SeedImageEditAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.ImageEdits);
            await dbContext.SaveChangesAsync();
        }
    }
}