using AI.Common.EFCore;
using Summary.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Summary.Data.Seed;

public class SummaryDataSeeder(
    SummaryDbContext eventDbContext,
    SummaryReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedSummaryAsync();
        }
    }

    private async Task SeedSummaryAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Summarys))
        {
            await eventDbContext.Summarys.AddRangeAsync(InitialData.Summarys);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Summarys.AsQueryable()))
            {
                await eventReadDbContext.Summarys.InsertManyAsync(mapper.Map<List<TextSummaryReadModel>>(InitialData.Summarys));
            }
        }
    }
}