using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Payment.Data.Seed;

public class PaymentDataSeeder(
    PaymentDbContext dbContext
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedPaymentAsync();
        }
    }

    private async Task SeedPaymentAsync()
    {
        if (!await dbContext.Sessions.AnyAsync())
        {
            await dbContext.Subscriptions.AddRangeAsync(InitialData.Subscriptions);
            await dbContext.SaveChangesAsync();
        }
    }
}