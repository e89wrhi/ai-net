using AI.Common.EFCore;
using TextToSpeech.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TextToSpeech.Data.Seed;

public class TextToSpeechDataSeeder(
    TextToSpeechDbContext eventDbContext,
    TextToSpeechReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedTextToSpeechAsync();
        }
    }

    private async Task SeedTextToSpeechAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.TextToSpeechs))
        {
            await eventDbContext.TextToSpeechs.AddRangeAsync(InitialData.TextToSpeechs);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.TextToSpeechs.AsQueryable()))
            {
                await eventReadDbContext.TextToSpeechs.InsertManyAsync(mapper.Map<List<TextToSpeechSessionReadModel>>(InitialData.TextToSpeechs));
            }
        }
    }
}