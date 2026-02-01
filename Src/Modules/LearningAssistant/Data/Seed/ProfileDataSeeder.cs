using AI.Common.EFCore;
using LearningAssistant.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LearningAssistant.Data.Seed;

public class ProfileDataSeeder(
    LearningDbContext eventDbContext,
    LearningReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedAssistantAsync();
        }
    }

    private async Task SeedAssistantAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Profiles))
        {
            await eventDbContext.Profiles.AddRangeAsync(InitialData.Profiles);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Profiles.AsQueryable()))
            {
                await eventReadDbContext.Profiles.InsertManyAsync(mapper.Map<List<LearningSessionReadModel>>(InitialData.Profiles));
            }
        }
    }
}