using AI.Common.EFCore;
using Meeting.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Meeting.Data.Seed;

public class MeetingDataSeeder(
    MeetingDbContext eventDbContext,
    MeetingReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedMeetingAsync();
        }
    }

    private async Task SeedMeetingAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Meetings))
        {
            await eventDbContext.Meetings.AddRangeAsync(InitialData.Meetings);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Meeting.AsQueryable()))
            {
                await eventReadDbContext.Meeting.InsertManyAsync(mapper.Map<List<MeetingAnalysisSessionReadModel>>(InitialData.Meetings));
            }
        }
    }
}