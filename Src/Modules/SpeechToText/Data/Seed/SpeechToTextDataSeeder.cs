using AI.Common.EFCore;
using SpeechToText.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace SpeechToText.Data.Seed;

public class SpeechToTextDataSeeder(
    SpeechToTextDbContext eventDbContext,
    SpeechToTextReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSpeechToTextAsync();
        }
    }

    private async Task SeedSpeechToTextAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.SpeechToTexts))
        {
            await eventDbContext.SpeechToTexts.AddRangeAsync(InitialData.SpeechToTexts);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.SpeechToTexts.AsQueryable()))
            {
                await eventReadDbContext.SpeechToTexts.InsertManyAsync(mapper.Map<List<SpeechToTextSessionReadModel>>(InitialData.SpeechToTexts));
            }
        }
    }
}