using AI.Common.EFCore;
using CodeDebug.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CodeDebug.Data.Seed;

public class CodeDebugDataSeeder(
    CodeDebugDbContext eventDbContext,
    CodeDebugReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedCodeDebugAsync();
        }
    }

    private async Task SeedCodeDebugAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.CodeDebugs))
        {
            await eventDbContext.CodeDebugs.AddRangeAsync(InitialData.CodeDebugs);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.CodeDebugs.AsQueryable()))
            {
                await eventReadDbContext.CodeDebugs.InsertManyAsync(mapper.Map<List<CodeDebugSessionReadModel>>(InitialData.CodeDebugs));
            }
        }
    }
}