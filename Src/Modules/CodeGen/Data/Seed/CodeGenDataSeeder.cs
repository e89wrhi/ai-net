using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace CodeGen.Data.Seed;

public class CodeGenDataSeeder(
    CodeGenDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedCodeGenAsync();
        }
    }

    private async Task SeedCodeGenAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.CodeGens);
            await dbContext.SaveChangesAsync();
        }
    }
}