using AI.Common.EFCore;
using AutoComplete.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AutoComplete.Data.Seed;

public class AutocompleteDataSeeder(
    AutocompleteDbContext eventDbContext,
    AutocompleteReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedAutoCompleteAsync();
        }
    }

    private async Task SeedAutoCompleteAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.AutoCompletes))
        {
            await eventDbContext.AutoCompletes.AddRangeAsync(InitialData.AutoCompletes);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.AutoCompletes.AsQueryable()))
            {
                await eventReadDbContext.AutoCompletes.InsertManyAsync(mapper.Map<List<AutoCompleteSessionReadModel>>(InitialData.AutoCompletes));
            }
        }
    }
}