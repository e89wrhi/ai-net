using AI.Common.EFCore;
using User.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace User.Data.Seed;

public class UserDataSeeder(
    UserDbContext eventDbContext,
    UserReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedUserAsync();
        }
    }

    private async Task SeedUserAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Users))
        {
            await eventDbContext.Users.AddRangeAsync(InitialData.Users);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.User.AsQueryable()))
            {
                await eventReadDbContext.User.InsertManyAsync(mapper.Map<List<UserReadModel>>(InitialData.Users));
            }
        }
    }
}