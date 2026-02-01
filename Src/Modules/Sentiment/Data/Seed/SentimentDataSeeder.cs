using AI.Common.EFCore;
using Sentiment.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Sentiment.Data.Seed;

public class SentimentDataSeeder(
    SentimentDbContext eventDbContext,
    SentimentReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSentimentAsync();
        }
    }

    private async Task SeedSentimentAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Sentiments))
        {
            await eventDbContext.Sentiments.AddRangeAsync(InitialData.Sentiments);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Sentiments.AsQueryable()))
            {
                await eventReadDbContext.Sentiments.InsertManyAsync(mapper.Map<List<TextSentimentReadModel>>(InitialData.Sentiments));
            }
        }
    }
}