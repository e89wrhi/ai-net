using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Resume.Data.Seed;

public class ResumeDataSeeder(
    ResumeDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedResumeAsync();
        }
    }

    private async Task SeedResumeAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Resumes);
            await dbContext.SaveChangesAsync();
        }
    }
}