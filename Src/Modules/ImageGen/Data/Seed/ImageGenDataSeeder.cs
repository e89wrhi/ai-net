using AI.Common.EFCore;
using ImageGen.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageGen.Data.Seed;

public class ImageGenDataSeeder(
    ImageGenDbContext eventDbContext,
    ImageGenReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageGenAsync();
        }
    }

    private async Task SeedImageGenAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.ImageGens))
        {
            await eventDbContext.ImageGens.AddRangeAsync(InitialData.ImageGens);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.ImageGens.AsQueryable()))
            {
                await eventReadDbContext.ImageGens.InsertManyAsync(mapper.Map<List<ImageGenerationReadModel>>(InitialData.ImageGens));
            }
        }
    }
}