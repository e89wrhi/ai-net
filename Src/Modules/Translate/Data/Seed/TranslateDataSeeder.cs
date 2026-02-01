using AI.Common.EFCore;
using Translate.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Translate.Data.Seed;

public class TranslateDataSeeder(
    TranslateDbContext eventDbContext,
    TranslateReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedTranslateAsync();
        }
    }

    private async Task SeedTranslateAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Translates))
        {
            await eventDbContext.Translates.AddRangeAsync(InitialData.Translates);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Translates.AsQueryable()))
            {
                await eventReadDbContext.Translates.InsertManyAsync(mapper.Map<List<TranslationSessionReadModel>>(InitialData.Translates));
            }
        }
    }
}