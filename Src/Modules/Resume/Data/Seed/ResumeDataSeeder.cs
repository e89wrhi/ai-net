using AI.Common.EFCore;
using Resume.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Resume.Data.Seed;

public class ResumeDataSeeder(
    ResumeDbContext eventDbContext,
    ResumeReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedResumeAsync();
        }
    }

    private async Task SeedResumeAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Resumes))
        {
            await eventDbContext.Resumes.AddRangeAsync(InitialData.Resumes);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Resume.AsQueryable()))
            {
                await eventReadDbContext.Resume.InsertManyAsync(mapper.Map<List<ResumeReadModel>>(InitialData.Resumes));
            }
        }
    }
}