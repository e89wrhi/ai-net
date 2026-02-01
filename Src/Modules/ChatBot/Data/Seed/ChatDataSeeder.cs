using AI.Common.EFCore;
using ChatBot.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ChatBot.Data.Seed;

public class ChatDataSeeder(
    ChatDbContext eventDbContext,
    ChatReadDbContext eventReadDbContext,
    IMapper mapper
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var pendingMigrations = await eventDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedChatAsync();
        }
    }

    private async Task SeedChatAsync()
    {
        if (!await EntityFrameworkQueryableExtensions.AnyAsync(eventDbContext.Chats))
        {
            await eventDbContext.Chats.AddRangeAsync(InitialData.Chats);
            await eventDbContext.SaveChangesAsync();

            if (!await MongoQueryable.AnyAsync(eventReadDbContext.Chats.AsQueryable()))
            {
                await eventReadDbContext.Chats.InsertManyAsync(mapper.Map<List<ChatSessionReadModel>>(InitialData.Chats));
            }
        }
    }
}