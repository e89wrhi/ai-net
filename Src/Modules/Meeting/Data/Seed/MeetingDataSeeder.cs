using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Meeting.Data.Seed;

public class MeetingDataSeeder(
    MeetingDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedMeetingAsync();
        }
    }

    private async Task SeedMeetingAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Sessions.AddRangeAsync(InitialData.Meetings);
            await dbContext.SaveChangesAsync();
        }
    }
}