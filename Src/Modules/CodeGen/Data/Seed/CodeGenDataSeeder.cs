using AI.Common.EFCore;
using CodeGen.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CodeGen.Data.Seed;

public class CodeGenDataSeeder(
    CodeGenDbContext eventDbContext,
    CodeGenReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedCodeGenAsync();
        }
    }

    private async Task SeedCodeGenAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.CodeGens))
        {
            await eventDbContext.CodeGens.AddRangeAsync(InitialData.CodeGens);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.CodeGens.AsQueryable()))
            {
                await eventReadDbContext.CodeGens.InsertManyAsync(mapper.Map<List<CodeGenerationReadModel>>(InitialData.CodeGens));
            }
        }
    }
}