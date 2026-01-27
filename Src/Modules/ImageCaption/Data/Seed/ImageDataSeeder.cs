using AI.Common.EFCore;
using ImageCaption.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageCaption.Data.Seed;

public class ImageDataSeeder(
    ImageDbContext eventDbContext,
    ImageReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedImageAsync();
        }
    }

    private async Task SeedImageAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Images))
        {
            await eventDbContext.Images.AddRangeAsync(InitialData.Images);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Image.AsQueryable()))
            {
                await eventReadDbContext.Image.InsertManyAsync(mapper.Map<List<ImageReadModel>>(InitialData.Images));
            }
        }
    }
}