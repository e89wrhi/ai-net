using AI.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Data.Seed;

public class ChatDataSeeder(
    ChatDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedChatAsync();
        }
    }

    private async Task SeedChatAsync()
    {
        if (!await dbContext.Chats.AnyAsync())
        {
            await dbContext.Chats.AddRangeAsync(InitialData.Chats);
            await dbContext.SaveChangesAsync();
        }
    }
}