using AI.Common.EFCore;
using AutoComplete.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoComplete.Data.Seed;

public class AutocompleteDataSeeder(
    AutocompleteDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedAutoCompleteAsync();
        }
    }

    private async Task SeedAutoCompleteAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.AutoCompletes);
            await dbContext.SaveChangesAsync();
        }
    }
}