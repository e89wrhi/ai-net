using AI.Common.EFCore;
using ImageEdit.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageEdit.Data.Seed;

public class ImageEditDataSeeder(
    ImageEditDbContext eventDbContext,
    ImageEditReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageEditAsync();
        }
    }

    private async Task SeedImageEditAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.ImageEdits))
        {
            await eventDbContext.ImageEdits.AddRangeAsync(InitialData.ImageEdits);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.ImageEdits.AsQueryable()))
            {
                await eventReadDbContext.ImageEdits.InsertManyAsync(mapper.Map<List<ImageEditSessionReadModel>>(InitialData.ImageEdits));
            }
        }
    }
}