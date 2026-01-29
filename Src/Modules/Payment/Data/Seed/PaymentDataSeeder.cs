using AI.Common.EFCore;
using Payment.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Payment.Data.Seed;

public class PaymentDataSeeder(
    PaymentDbContext eventDbContext,
    PaymentReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedPaymentAsync();
        }
    }

    private async Task SeedPaymentAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Subscriptions))
        {
            await eventDbContext.Subscriptions.AddRangeAsync(InitialData.Subscriptions);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Subscription.AsQueryable()))
            {
                await eventReadDbContext.Subscription.InsertManyAsync(mapper.Map<List<SubscriptionReadModel>>(InitialData.Subscriptions));
            }
        }
    }
}